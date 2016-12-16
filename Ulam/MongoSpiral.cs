using Euler;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using EulerMath;
using MongoDB.Bson;

namespace Ulam
{
    public class MongoSpiral : ISpiral
    {
        private readonly long _max;
        private readonly IMongoCollection<UlamElement> _map;
        private readonly MongoClientSettings _clientSettings;

        public MongoSpiral(long max = long.MaxValue)
        {
            _max = max;
            _clientSettings = new MongoClientSettings
            {
                // this is the default setting (100)
                //MaxConnectionPoolSize = int.MaxValue
            };
            var client = new MongoClient(_clientSettings);
            _map = client.GetDatabase("Ulam").GetCollection<UlamElement>("map");
        }

        /// <summary>
        /// Generates a Ulam spiral and persists it to a MongoDB collection
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task GenerateAndSave(string file = null)
        {
            long initialRoot = 3;
            long minK = (long)Math.Floor((initialRoot + 1) / 2d) + 1;
            long maxK = (long)Math.Sqrt(_max) / 2;

            NonBlockingConsole.WriteLine("Initializing...");

            var countTask = Count();

            var opt = new FindOptions<UlamElement>
            {
                Sort = new JsonSortDefinition<UlamElement>("{ Value : -1 }")
            };
            var cursor = _map.FindAsync(JsonFilterDefinition<UlamElement>.Empty, opt);

            await Task.WhenAll(cursor, countTask);

            var count = countTask.Result;
            long last = 0;
            bool foundTheEnd = false;
            foreach (var largestNumber in cursor.Result.ToEnumerable())
            {
                NonBlockingConsole.WriteLine("Stepping back to " + largestNumber.Value + " because count is " + count);

                if (count < largestNumber.Value)
                {
                    if (last > 0)
                    {
                        var diff = last - largestNumber.Value;
                        count -= diff;
                    }

                    last = largestNumber.Value;
                }
                else
                {
                    long root = (long)Math.Floor(Math.Sqrt(largestNumber.Value));

                    // make the root odd
                    if (root % 2 == 0)
                    {
                        root--;
                    }

                    minK = (root - 1) / 2;

                    foundTheEnd = true;
                    break;
                }
            }

            if (!foundTheEnd)
            {
                try
                {
                    // clear out the collection. This is less than 9 documents
                    var result = await _map.DeleteManyAsync(FilterDefinition<UlamElement>.Empty);
                    NonBlockingConsole.WriteLine("Deleted " + result.DeletedCount + " documents");

                    // seed up to 9
                    var seeds = new[]
                    {
                        new UlamElement(1, 0, 0, false),
                        new UlamElement(2, 1, 0, true),
                        new UlamElement(3, 1, 1, true),
                        new UlamElement(4, 0, 1, false),
                        new UlamElement(5, -1, 1, true),
                        new UlamElement(6, -1, 0, false),
                        new UlamElement(7, -1, -1, true),
                        new UlamElement(8, 0, -1, false),
                        new UlamElement(9, 1, -1, false)
                    };

                    await _map.InsertManyAsync(seeds);

                    minK = 1;
                }
                catch (MongoWriteException ex)
                {
                    if (ex.WriteError.Category != ServerErrorCategory.DuplicateKey)
                    {
                        throw;
                    }
                }
            }

            minK = Math.Max(1, minK);
            for (long k = minK; k <= maxK; k++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                long failures = 0;
                long dups = 0;

                long localMax = (2 * k + 3) * (2 * k + 3);
                count = await Count(localMax);

                var cache = new HashSet<UlamElement>();

                while (count < localMax)
                {
                    NonBlockingConsole.WriteLine();
                    if (failures > 1)
                    {
                        foreach (var element in cache)
                        {
                            try
                            {
                                _map.InsertOne(element);
                            }
                            catch (MongoWriteException ex)
                            {
                                if (ex.WriteError.Category != ServerErrorCategory.DuplicateKey)
                                {
                                    throw;
                                }

                                dups++;
                            }
                            catch (Exception ex)
                            {
                                //NonBlockingConsole.WriteLine(element.ToJson());
                                //NonBlockingConsole.WriteLine(ex);

                                failures++;
                            }
                        }
                    }
                    else {
                        using (var queue = new BlockingCollection<UlamElement>())
                        {
                            // produce
                            Task producer = Produce(k, queue);

                            var options = new ParallelOptions();
                            options.MaxDegreeOfParallelism = _clientSettings.MaxConnectionPoolSize;

                            // consume
                            Parallel.ForEach(queue.GetConsumingPartitioner(), options, element =>
                            {
                                try
                                {
                                    element.IsPrime = IsPrime(element);
                                    cache.Add(element);

                                    try
                                    {
                                        _map.InsertOne(element);
                                    }
                                    catch (MongoWriteException ex)
                                    {
                                        if (ex.WriteError.Category != ServerErrorCategory.DuplicateKey)
                                        {
                                            throw;
                                        }

                                        dups++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //NonBlockingConsole.WriteLine(element.ToJson());
                                    //NonBlockingConsole.WriteLine(ex);

                                    failures++;
                                }
                            });

                            await producer;
                        }
                    }

                    count = await Count(localMax);
                }

                stopwatch.Stop();

                long root = 2 * k + 1;
                long p = root * root;
                long min = p + 1;
                long max = (root + 2) * (root + 2);
                var diff = max - min;
                var timeSpan = stopwatch.Elapsed.ToHumanReadableString();
                var rate = diff / stopwatch.Elapsed.TotalSeconds;

                var message = "                     | "
                              + "Root".PadRight(root.ToString().Length) + " | "
                              + "Min".PadRight(min.ToString().Length) + " | "
                              + "Max".PadRight(max.ToString().Length) + " | "
                              + "Delta".PadRight(diff.ToString().Length) + " | "
                              //+ "To Go".PadRight(toGo.ToString().Length) + " | "
                              + "Elapsed".PadRight(timeSpan.Length) + " | "
                              + "Rate".PadRight(rate.ToString("F3").Length + 4) + " | "
                              + "Fails".PadRight(failures.ToString().Length) + " | "
                              + "Dups".PadRight(dups.ToString().Length)
                              + "\r\n" + DateTime.Now.ToString("MM/dd/yy HH:mm:ss.ff") + " | "
                              + root.ToString().PadRight(4) + " | "
                              + min.ToString().PadRight(3) + " | "
                              + max.ToString().PadRight(3) + " | "
                              + diff.ToString().PadRight(5) + " | "
                              + timeSpan.PadRight(7) + " | "
                              + rate.ToString("F3").PadRight(4) + " #/s | "
                              + failures.ToString().PadRight(5) + " | "
                              + dups.ToString().PadRight(4);

                NonBlockingConsole.WriteLine(message);
            }
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
            if (p % 2 == 0 || p % 3 == 0 || p % 5 == 0 || p % 7 == 0)
            {
                // this eliminates a lot of numbers without looking to the database
                return false;
            }

            // this is an odd perfect square of a prime,  we would have to scan  up to Sqrt(p) to find the divisor, so we quit early with this easy check
            if (element.Location.X > 0 && element.Location.X == -1 * element.Location.Y)
            {
                return false;
            }

            long max = (long)Math.Ceiling(Math.Sqrt(p));
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

        private async Task<long> Count(long lte = 0)
        {
            FilterDefinition<UlamElement> filter;
            if (lte > 0)
            {
                filter = new JsonFilterDefinition<UlamElement>("{ Value : { $lte : " + lte + " } }");
            }
            else
            {
                filter = FilterDefinition<UlamElement>.Empty;
            }

            return await _map.CountAsync(filter);
        }


        private async Task<UlamElement> GetMax(long lte = 0)
        {
            FilterDefinition<UlamElement> filter;
            if (lte > 0)
            {
                filter = new JsonFilterDefinition<UlamElement>("{ Value : { $lte : " + lte + " } }");
            }
            else
            {
                filter = FilterDefinition<UlamElement>.Empty;
            }
            var options = new FindOptions<UlamElement, UlamElement>
            {
                Sort = new JsonSortDefinition<UlamElement>("{ Value : -1 }"),
                Limit = 1
            };
            var cursor = await _map.FindAsync(filter, options);
            var largestNumber = await cursor.FirstOrDefaultAsync();

            return largestNumber;
        }

        /// <summary>
        /// Produces one loop of the spiral and writes it to the queue.
        /// 2<paramref name="k"/>+1)^2 $lt x $lte (2(<paramref name="k"/>+1)+1)^2
        /// </summary>
        /// <param name="k"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
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