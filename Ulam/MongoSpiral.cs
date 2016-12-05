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
using System.Diagnostics;

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

            long initialRoot = 3;
            long minK = (long)Math.Floor((initialRoot + 1) / 2d) + 1;
            long maxK = (long)Math.Sqrt((double)_max) / 2;

            await Task.WhenAll(t1, t2);

            for (long k = minK; k <= maxK; k++)
            {
                using (var queue = new BlockingCollection<UlamElement>())
                {
                    var producer = Task.Run(() =>
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();

                        long root = 2 * k + 1;
                        long p = root * root;
                        long x = root - 2;
                        long y = -x;
                        long sideLength = root;

                        root += 2;
                        long min = p + 1;
                        long max = root * root;
                        long diff = max - min;
                        long toGo = _max - p;

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

                        stopwatch.Stop();
                        var message = "Time Stamp             " + " | "
                                    + "Root".PadRight(root.ToString().Length) + " | "
                                    + "Min".PadRight(min.ToString().Length) + " | "
                                    + "Max".PadRight(max.ToString().Length) + " | "
                                    + "Step Size".PadRight(diff.ToString().Length) + " | "
                                    + "Limit".PadRight(_max.ToString().Length) + " | "
                                    + "Elapsed".PadRight(stopwatch.Elapsed.TotalMilliseconds.ToString().Length + 2)
                                    + "\r\n" + DateTime.Now.ToString("MM/dd/yy H:mm:ss.ffff") + " | "
                                    + root.ToString().PadRight(4) + " | "
                                    + min.ToString().PadRight(3) + " | "
                                    + max.ToString().PadRight(3) + " | "
                                    + diff.ToString().PadRight(9) + " | "
                                    + _max.ToString().PadRight(5) + " | "
                                    + stopwatch.Elapsed.TotalMilliseconds.ToString().PadRight(7) + "ms";
                        NonBlockingConsole.WriteLine(message);
                    });

                    Parallel.ForEach(queue.GetConsumingEnumerable(), new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = _clientSettings.MaxConnectionPoolSize
                    }, (doc) =>
                    {
                        _map.InsertOne(doc);
                    });

                    await producer;
                }
            }
        }
    }
}