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
        }

        public async Task GenerateAndSave(string file = null)
        {
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

            long initialRoot = 3;
            long minK = (long)Math.Floor((initialRoot + 1) / 2d) + 1;
            long maxK = (long)Math.Sqrt((double)_max) / 2;

            if (largestNumber == null || largestNumber.Value <= 9)
            {
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

                var insert = _map.InsertManyAsync(seeds);

                // clear out the collection. This is less than 9 documents
                var result = await _map.DeleteManyAsync(FilterDefinition<UlamElement>.Empty);
                NonBlockingConsole.WriteLine("Deleted " + result.DeletedCount + " documents");

                await insert;
            }
            else
            {
                using (var cursor = await _map.FindAsync(FilterDefinition<UlamElement>.Empty, options))
                {
                    long root = (long)(Math.Floor(Math.Sqrt(largestNumber.Value)));

                    // remove the last ring to be safe 
                    if (root % 2 == 0)
                    {
                        root--;
                    }
                    else
                    {
                        root -= 2;
                    }

                    long p = root * root;
                    var result = await _map.DeleteManyAsync(new JsonFilterDefinition<UlamElement>("{ Value : { $gt : " + p + " } }"));
                    NonBlockingConsole.WriteLine("Deleted " + result.DeletedCount + " documents");

                    minK = (root - 1) / 2;
                }
            }

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
                        var message = "Time Stamp          " + " | "
                                    + "Root".PadRight(root.ToString().Length) + " | "
                                    + "Min".PadRight(min.ToString().Length) + " | "
                                    + "Max".PadRight(max.ToString().Length) + " | "
                                    + "Delta".PadRight(diff.ToString().Length) + " | "
                                    + "To Go".PadRight(toGo.ToString().Length) + " | "
                                    + "Elapsed".PadRight(stopwatch.Elapsed.TotalMilliseconds.ToString().Length + 2)
                                    + "\r\n" + DateTime.Now.ToString("MM/dd/yy H:mm:ss.ff") + " | "
                                    + root.ToString().PadRight(4) + " | "
                                    + min.ToString().PadRight(3) + " | "
                                    + max.ToString().PadRight(3) + " | "
                                    + diff.ToString().PadRight(5) + " | "
                                    + toGo.ToString().PadRight(5) + " | "
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