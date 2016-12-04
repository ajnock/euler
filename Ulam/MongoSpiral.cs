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
        private static readonly long OneAsLong = ((ulong)1).ToLong();
        private readonly ulong _max;
        private readonly IMongoCollection<UlamElement> _map;
        private readonly IMongoCollection<UlamElement> _primes;
        private readonly MongoClientSettings _clientSettings;

        public MongoSpiral(ulong max)
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
            ulong p = 1;
            long x = 0;
            long y = 0;
            long sideLength = 1;

            // build cache
            var primesOptions = new FindOptions<UlamElement, UlamElement>()
            {
                Sort = new JsonSortDefinition<UlamElement>("{ LongValue : -1 }"),
                Limit = int.MaxValue
            };

            // find the largest number
            UlamElement largestNumber;
            var options = new FindOptions<UlamElement, UlamElement>()
            {
                Sort = new JsonSortDefinition<UlamElement>("{ LongValue : -1 }"),
                Limit = 1
            };
            using (var cursor = await _map.FindAsync(FilterDefinition<UlamElement>.Empty, options))
            {
                largestNumber = await cursor.FirstOrDefaultAsync();
            }

            if (largestNumber.Value < 9)
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
                            new UlamElement(9, 1, -1, false),
                        };

                var inserts = new List<Task>();
                foreach (var seed in seeds.Where(s => s.IsPrime))
                {
                    inserts.Add(_map.InsertOneAsync(seed));
                    inserts.Add(_primes.InsertOneAsync(seed));
                }

                p = 9;
                x = 1;
                y = -1;
                sideLength = 3;

                await Task.WhenAll(inserts);
            }
            else
            {
                using (var cursor = await _map.FindAsync(FilterDefinition<UlamElement>.Empty, options))
                {
                    long root = (long)Math.Floor(Math.Sqrt(largestNumber.Value));

                    // remove the last ring to be safe
                    root--;

                    // make it odd
                    if (root % 2 == 0)
                    {
                        root--;
                    }

                    p = (ulong)root * (ulong)root;

                    long deleted = 0;
                    var count = _map.Count(FilterDefinition<UlamElement>.Empty);

                    while ((ulong)count != p)
                    {
                        root -= 2;
                        p = (ulong)root * (ulong)root;

                        var result1 = _map.DeleteManyAsync(new JsonFilterDefinition<UlamElement>("{ LongValue : { $gt : " + p.ToLong() + " } }"));
                        var result2 = _primes.DeleteManyAsync(new JsonFilterDefinition<UlamElement>("{ LongValue : { $gt : " + p.ToLong() + " } }"));

                        await Task.WhenAll(result1, result2);

                        deleted += result1.Result.DeletedCount;
                        deleted += result2.Result.DeletedCount;
                        NonBlockingConsole.WriteLine("  -" + result1.Result.DeletedCount + " numbers");
                        NonBlockingConsole.WriteLine("  -" + result2.Result.DeletedCount + " primes");

                        count = _map.Count(FilterDefinition<UlamElement>.Empty);
                    }

                    NonBlockingConsole.WriteLine("Deleted " + deleted + " total documents");

                    x = root;
                    y = -x;
                    sideLength = 2 * x;
                }
            }

            while (p < _max)
            {
                var queue = new BlockingCollection<Task<UlamElement>>(_clientSettings.MaxConnectionPoolSize - 2);
                var primes = new BlockingCollection<UlamElement>();

                NonBlockingConsole.WriteLine("Side Length: " + sideLength);

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
                                NonBlockingConsole.WriteLine(entry.Value);
                                primes.Add(entry);
                            }

                            _map.InsertOne(entry);
                        }
                    }

                    primes.CompleteAdding();

                    //NonBlockingConsole.WriteLine("Ulam Consumer");
                });

                var primesConsumer = Task.Run(() =>
                {
                    foreach (var prime in primes.GetConsumingEnumerable())
                    {
                        _primes.InsertOne(prime);
                    }

                    //NonBlockingConsole.WriteLine("Primes Consumer");
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

                    //NonBlockingConsole.WriteLine("Producer");
                });

                await Task.WhenAll(producer, ulamConsumer, primesConsumer);
            }
        }

        private async Task<UlamElement> NewEntry(ulong p, long x, long y)
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
                ulong third = (ulong)(p / 3) + 1;
                long value = third.ToLong();
                using (var cursor = await _primes.FindAsync(new JsonFilterDefinition<UlamElement>("{ $and : [ { LongValue : { $gt : " + OneAsLong + " } }, { LongValue : { $lte : " + value + " } } ] }")))
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