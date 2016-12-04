using Euler;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EulerMath;
using System.Threading;
using System.Collections.Concurrent;
using MongoDB.Bson.Serialization.Attributes;

namespace Ulam
{
    public class MongoSpiral : ISpiral
    {
        private readonly long _max;
        private readonly IMongoCollection<UlamElement> _map;
        private readonly IMongoCollection<UlamElement> _primes;
        private readonly MongoClientSettings _clientSettings;

        public MongoSpiral(long max)
        {
            _max = max;
            _clientSettings = new MongoClientSettings()
            {
                // this is the default setting (100)
                MaxConnectionPoolSize = 100
            };
            var client = new MongoClient(_clientSettings);
            _map = client.GetDatabase("Ulam").GetCollection<UlamElement>("map");
            _primes = client.GetDatabase("Ulam").GetCollection<UlamElement>("primes");
        }

        public async Task GenerateAndSave(string file = null)
        {
            long p = 1;
            long root = (long)Math.Sqrt(p);
            long lastP = p;
            long x = 0;
            long y = 0;
            long sideLength = 1;

            // build cache
            var primesOptions = new FindOptions<UlamElement, UlamElement>()
            {
                Sort = new JsonSortDefinition<UlamElement>("{ Value : -1 }"),
                Limit = int.MaxValue
            };

            // find the largest number
            UlamElement largestNumber;
            var options = new FindOptions<UlamElement, UlamElement>()
            {
                Sort = new JsonSortDefinition<UlamElement>("{ Value : -1 }"),
                Limit = 1
            };
            using (var cursor = await _map.FindAsync(FilterDefinition<UlamElement>.Empty, options))
            {
                largestNumber = await cursor.FirstOrDefaultAsync();
            }

            if (largestNumber == null || largestNumber.Value <= 9)
            {
                // clear out the collection. This is less than 9 documents
                var result1 = _map.DeleteManyAsync(FilterDefinition<UlamElement>.Empty);
                var result2 = _primes.DeleteManyAsync(FilterDefinition<UlamElement>.Empty);

                await Task.WhenAll(result1, result2);

                var deleted = result1.Result.DeletedCount;
                deleted += result2.Result.DeletedCount;
                NonBlockingConsole.WriteLine("  -" + result1.Result.DeletedCount + " numbers");
                NonBlockingConsole.WriteLine("  -" + result2.Result.DeletedCount + " primes");
                NonBlockingConsole.WriteLine("Deleted " + deleted + " total documents");

                // seed up to 9
                var seeds = new UlamElement[] {
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

                var t1 = _map.InsertManyAsync(seeds);
                var t2 = _primes.InsertManyAsync(seeds.Where(s => s.IsPrime));

                p = 9;
                root = 3;
                lastP = 1;
                x = 1;
                y = -1;
                sideLength = 3;

                await Task.WhenAll(t1, t2);
            }
            else
            {
                using (var cursor = await _map.FindAsync(FilterDefinition<UlamElement>.Empty, options))
                {
                    root = (long)(Math.Floor(Math.Sqrt(largestNumber.Value)));

                    // remove the last ring to be safe 
                    if (root % 2 == 0)
                    {
                        root -= 3;
                    }
                    else
                    {
                        root -= 2;
                    }

                    p = root * root;
                    x = root - 2;
                    y = -x;
                    sideLength = root;

                    long deleted = 0;
                    var result1 = _map.DeleteManyAsync(new JsonFilterDefinition<UlamElement>("{ Value : { $gt : " + p + " } }"));
                    var result2 = _primes.DeleteManyAsync(new JsonFilterDefinition<UlamElement>("{ Value : { $gt : " + p + " } }"));

                    await Task.WhenAll(result1, result2);

                    deleted += result1.Result.DeletedCount;
                    deleted += result2.Result.DeletedCount;
                    NonBlockingConsole.WriteLine("  -" + result1.Result.DeletedCount + " numbers");
                    NonBlockingConsole.WriteLine("  -" + result2.Result.DeletedCount + " primes");
                    NonBlockingConsole.WriteLine("Deleted " + deleted + " total documents");
                }
            }

            while (p < _max)
            {
                root += 2;
                long min = p + 1;
                long max = root * root;
                long diff = max - min;

                NonBlockingConsole.WriteLine();
                NonBlockingConsole.WriteLine("                      " + " | Root".PadRight(root.ToString().Length) + " | Current".PadRight(min.ToString().Length) + " | Max".PadRight(max.ToString().Length) + " | Step Size".PadRight(diff.ToString().Length));
                NonBlockingConsole.WriteLine(DateTime.Now.ToString("MM/dd/yy H:mm:ss.ffff") + " | " + root + " | " + min + " | " + max + " | " + diff);

                var queue = new BlockingCollection<Task<UlamElement>>(_clientSettings.MaxConnectionPoolSize - 2);
                var primes = new BlockingCollection<UlamElement>();

                var ulamConsumer = Task.Run(() =>
                {
                    while (!queue.IsCompleted)
                    {
                        Task<UlamElement> task;
                        while (queue.TryTake(out task))
                        {
                            var entry = task.Result;

                            if (entry.IsPrime)
                            {
                                NonBlockingConsole.Write(".");
                                primes.Add(entry);
                            }

                            _map.InsertOne(entry);
                        }
                    }

                    primes.CompleteAdding();
                });

                var primesConsumer = Task.Run(() =>
                {
                    foreach (var prime in primes.GetConsumingEnumerable())
                    {
                        _primes.InsertOne(prime);
                    }
                });

                var producer = Task.Run(() =>
                {
                    x++;
                    p++;
                    queue.Add(NewEntry(p, x, y));

                    for (long i = 0; i < sideLength; i++)
                    {
                        y++;
                        p++;
                        queue.Add(NewEntry(p, x, y));
                    }

                    sideLength++;
                    for (long i = 0; i < sideLength && p < _max; i++)
                    {
                        x--;
                        p++;

                        queue.Add(NewEntry(p, x, y));
                    }

                    for (long i = 0; i < sideLength && p < _max; i++)
                    {
                        y--;
                        p++;

                        queue.Add(NewEntry(p, x, y));
                    }

                    for (long i = 0; i < sideLength && p < _max; i++)
                    {
                        x++;
                        p++;

                        queue.Add(NewEntry(p, x, y));
                    }

                    sideLength++;

                    queue.CompleteAdding();
                });

                await Task.WhenAll(producer, ulamConsumer, primesConsumer);
            }
        }

        private async Task<UlamElement> NewEntry(long p, long x, long y)
        {
            var entry = new UlamElement(p, x, y);

            bool isPrime = true;

            // pre-seeded values
            if (p == 1)
            {
                isPrime = false;
            }
            else if (p == 2 || p == 3 || p == 5 || p == 7)
            {
                isPrime = true;
            }
            else if (p % 2 == 0 || p % 3 == 0 || p % 5 == 0 || p % 7 == 0)
            {
                isPrime = false;
            }
            else
            {
                long third = (p / 3) + 1;
                using (var cursor = await _primes.FindAsync(new JsonFilterDefinition<UlamElement>("{ Value : { $lte : " + third + " } }")))
                {
                    using (var source = new CancellationTokenSource())
                    {
                        var token = source.Token;

                        try
                        {
                            await cursor.ForEachAsync((e) =>
                            {
                                if (p % e.Value == 0)
                                {
                                    isPrime = false;
                                    source.Cancel();
                                }
                            }, source.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            isPrime = false;
                        }
                    }
                }
            }

            entry.IsPrime = isPrime;

            return entry;
        }
    }
}