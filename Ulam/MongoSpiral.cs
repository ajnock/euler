using Euler;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EulerMath;
using MongoDB.Bson;

namespace Ulam
{
    public class MongoSpiral : ISpiral
    {
        private readonly long _max;
        private readonly IMongoCollection<UlamElement> _map;
        //private readonly IMongoCollection<UlamElement> _primes;
        private readonly MongoClientSettings _clientSettings;

        private static readonly int ConsumingThreads = 2;
        private static readonly int ProducingThreads = 100;

        public MongoSpiral(long max = long.MaxValue)
        {
            _max = max;
            _clientSettings = new MongoClientSettings
            {
                // this is the default setting (100)
                MaxConnectionPoolSize = 100
            };
            var client = new MongoClient(_clientSettings);
            _map = client.GetDatabase("Ulam").GetCollection<UlamElement>("map");
            //_map = client.GetDatabase("Ulam").GetCollection<UlamElement>("primes");
        }

        public async Task GenerateAndSave(string file = null)
        {
            long initialRoot = 3;
            long minK = (long)Math.Floor((initialRoot + 1) / 2d) + 1;
            long maxK = (long)Math.Sqrt(_max) / 2;

            var largestNumber = await GetLargestNumber();

            if (largestNumber != null)
            {
                long deleted = 0;
                long count = await _map.CountAsync(FilterDefinition<UlamElement>.Empty);
                while (count != largestNumber.Value)
                {
                    var top = Math.Min(count, largestNumber.Value);
                    var deleteResult = await _map.DeleteManyAsync(new JsonFilterDefinition<UlamElement>("{ Value : { $gt : " + top + " } }"));

                    NonBlockingConsole.WriteLine("Deleted " + deleteResult.DeletedCount + " documents because " + count + " != " + largestNumber.Value);
                    deleted += deleteResult.DeletedCount;

                    var t1 = GetLargestNumber();
                    var t2 = _map.CountAsync(FilterDefinition<UlamElement>.Empty);

                    largestNumber = await t1;
                    count = await t2;
                }

                long root = (long)Math.Floor(Math.Sqrt(largestNumber.Value));

                // make the root odd
                if (root % 2 == 0)
                {
                    root--;
                }

                long p = root * root;
                var result = await _map.DeleteManyAsync(new JsonFilterDefinition<UlamElement>("{ Value : { $gt : " + p + " } }"));
                var task = GetLargestNumber();

                NonBlockingConsole.WriteLine("Deleted " + result.DeletedCount + " documents to get to square " + p);
                deleted += result.DeletedCount;
                NonBlockingConsole.WriteLine("Deleted " + deleted + " total documents");

                minK = (root - 1) / 2;
                largestNumber = await task;
            }

            if (largestNumber == null || largestNumber.Value <= 9)
            {
                // clear out the collection. This is less than 9 documents
                var result = await _map.DeleteManyAsync(FilterDefinition<UlamElement>.Empty);
                NonBlockingConsole.WriteLine("Deleted " + result.DeletedCount + " documents");

                // seed up to 9
                var seeds = new[] {
                            new UlamElement(1, 0, 0, false),
                            new UlamElement(2, 1, 0, true),
                            new UlamElement(3, 1, 1, true),
                            new UlamElement(4, 0, 1, false),
                            new UlamElement(5, -1, 1, true),
                            new UlamElement(6, -1, 0, false),
                            new UlamElement(7, -1, -1, true),
                            new UlamElement(8, 0, -1, false),
                            new UlamElement(9,1,-1,false)
                        };

                await _map.InsertManyAsync(seeds);

                minK = 1;
            }

            minK = Math.Max(1, minK);
            for (long k = minK; k <= maxK; k++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using (var queue = new BlockingCollection<UlamElement>())
                using (var queue2 = new BlockingCollection<UlamElement>())
                {
                    Task producer = Produce(k, queue);
                    Task primer = Prime(queue, queue2);
                    Task consumer = Consume(queue2);

                    await Task.WhenAll(producer, primer, consumer);
                }

                stopwatch.Stop();

                long root = 2 * k + 1;
                long p = root * root;
                long min = p + 1;
                long max = (root + 2) * (root + 2);
                long diff = max - min;
                long toGo = _max - p;
                var timeSpan = stopwatch.Elapsed.ToHumanReadableString();

                var message = "Time Stamp          " + " | "
                              + "Root".PadRight(root.ToString().Length) + " | "
                              + "Min".PadRight(min.ToString().Length) + " | "
                              + "Max".PadRight(max.ToString().Length) + " | "
                              + "Delta".PadRight(diff.ToString().Length) + " | "
                              + "To Go".PadRight(toGo.ToString().Length) + " | "
                              + "Elapsed".PadRight(timeSpan.Length)
                              + "\r\n" + DateTime.Now.ToString("MM/dd/yy H:mm:ss.ff") + " | "
                              + root.ToString().PadRight(4) + " | "
                              + min.ToString().PadRight(3) + " | "
                              + max.ToString().PadRight(3) + " | "
                              + diff.ToString().PadRight(5) + " | "
                              + toGo.ToString().PadRight(5) + " | "
                              + timeSpan.PadRight(7);

                NonBlockingConsole.WriteLine(message);
            }
        }

        private async Task Consume(BlockingCollection<UlamElement> queue2)
        {
            foreach (var doc in queue2.GetConsumingEnumerable())
            {
                _map.InsertOne(doc);
            }
        }

        private async Task Prime(BlockingCollection<UlamElement> inQueue, BlockingCollection<UlamElement> outQueue)
        {
            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = _clientSettings.MaxConnectionPoolSize - 1;

            Parallel.ForEach(inQueue.GetConsumingEnumerable(), options, element =>
            {
                element.IsPrime = IsPrime(element);
                outQueue.Add(element);
            });

            outQueue.CompleteAdding();
        }

        private bool IsPrime(UlamElement element)
        {
            var p = element.Value;
            if (p == 1)
            {
                // one is a special case
                return false;
            }
            if (p == 2 || p == 3)
            {
                return true;
            }
            if (p % 2 == 0 || p % 3 == 0)
            {
                // this eliminates a lot of numbers without looking to the database
                return false;
            }

            // this is an odd perfect square of a prime,  we would have to scan  up to Sqrt(p) to find the divisor, so we quit early with this easy check
            if (element.Location.X > 0 && element.Location.X == -1 * element.Location.Y)
            {
                return false;
            }

            long max = (long)Math.Ceiling(p / 5d);
            var filter = new JsonFilterDefinition<UlamElement>("{ $and : [ { IsPrime : true }, { Value : { $lte : " + max + " } } ] }");

            var cursor = _map.FindSync(filter);

            foreach (var prime in cursor.ToEnumerable())
            {
                var mod = p % prime.Value;
                if (mod == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<UlamElement> GetLargestNumber()
        {
            var options = new FindOptions<UlamElement, UlamElement>
            {
                Sort = new JsonSortDefinition<UlamElement>("{ Value : -1 }"),
                Limit = 1
            };
            var cursor = await _map.FindAsync(FilterDefinition<UlamElement>.Empty, options);
            var largestNumber = await cursor.FirstOrDefaultAsync();
            return largestNumber;
        }

        private async Task Produce(long k, BlockingCollection<UlamElement> queue)
        {
            long root = 2 * k + 1;
            long p = root * root;
            long x = k;
            long y = -x;
            long sideLength = 1 + 2 * k;

            x++;
            p++;
            queue.Add(new UlamElement(p, x, y));

            for (long i = 0; i < sideLength; i++)
            {
                y++;
                p++;
                queue.Add(new UlamElement(p, x, y));
            }

            sideLength++;
            for (long i = 0; i < sideLength && p < _max; i++)
            {
                x--;
                p++;
                queue.Add(new UlamElement(p, x, y));
            }

            for (long i = 0; i < sideLength && p < _max; i++)
            {
                y--;
                p++;
                queue.Add(new UlamElement(p, x, y));
            }

            for (long i = 0; i < sideLength && p < _max; i++)
            {
                x++;
                p++;
                queue.Add(new UlamElement(p, x, y));
            }

            queue.CompleteAdding();
        }
    }
}