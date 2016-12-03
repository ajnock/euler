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
        private readonly IMongoCollection<Entry> _entries;
        private readonly MongoClientSettings _clientSettings = new MongoClientSettings();

        public MongoSpiral(ulong max)
        {
            _max = max;
            var client = new MongoClient(_clientSettings);
            _entries = client.GetDatabase("Ulam").GetCollection<Entry>("map");
        }

        public async Task GenerateAndSave(string file)
        {
            try
            {
                ulong p = 1;
                long x = 0;
                long y = 0;
                long sideLength = 1;

                var options = new FindOptions<Entry, Entry>()
                {
                    Sort = new JsonSortDefinition<Entry>("{ LongValue : -1 }"),
                    Limit = 1
                };
                using (var cursor = await _entries.FindAsync(FilterDefinition<Entry>.Empty, options))
                {
                    var largestNumber = await cursor.FirstOrDefaultAsync();

                    if (largestNumber == null || largestNumber.Value < 9)
                    {
                        // clear out the collection. This is less than 9 documents
                        var result = await _entries.DeleteManyAsync(FilterDefinition<Entry>.Empty);
                        Console.WriteLine("Deleted " + result.DeletedCount + " documents");

                        // seed up to 9
                        var seeds = new Entry[] {
                            new Entry(1, 0, 0, false),
                            new Entry(2, 1, 0, true),
                            new Entry(3, 1, 1, true),
                            new Entry(4, 0, 1, false),
                            new Entry(5, -1, 1, true),
                            new Entry(6, -1, 0, false),
                            new Entry(7, -1, -1, true),
                            new Entry(8, 0, -1, false),
                            new Entry(9, 1, -1, false),
                        };

                        _entries.InsertMany(seeds);

                        p = 9;
                        x = 1;
                        y = -1;
                        sideLength = 3;
                    }
                    else
                    {
                        long root = (long)Math.Floor(Math.Sqrt(largestNumber.Value));
                        if (root % 2 == 0)
                        {
                            root--;
                        }

                        p = (ulong)root * (ulong)root;

                        var count = await _entries.CountAsync(FilterDefinition<Entry>.Empty);

                        long deleted = 0;
                        while ((ulong)count != p)
                        {
                            root -= 2;
                            p = (ulong)root * (ulong)root;

                            var result = await _entries.DeleteManyAsync(new JsonFilterDefinition<Entry>("{ LongValue : { $gt : " + p.ToLong() + " } }"));
                            deleted += result.DeletedCount;
                            Console.WriteLine("  -" + result.DeletedCount + " documents");

                            count = await _entries.CountAsync(FilterDefinition<Entry>.Empty);
                        }

                        NonBlockingConsole.WriteLine("Deleted " + deleted + " total documents");

                        x = root;
                        y = -x;
                        sideLength = 2 * x;
                    }
                }

                while (p <= _max)
                {
                    var queue = new BlockingCollection<Task<Entry>>(_clientSettings.MaxConnectionPoolSize - 1);
                    NonBlockingConsole.WriteLine("Side Length: " + sideLength);

                    var consumer = Task.Run(() =>
                    {
                        while (!queue.IsCompleted)
                        {
                            Task<Entry> task;
                            while (queue.TryTake(out task))
                            {
                                var entry = task.Result;

                                if (entry.IsPrime)
                                {
                                    NonBlockingConsole.WriteLine(entry.Value);
                                }

                                _entries.InsertOne(entry);
                            }
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
                        for (long i = 0; i < sideLength; i++)
                        {
                            x--;
                            p++;

                            queue.Add(NewEntry(p, x, y));
                        }

                        for (long i = 0; i < sideLength; i++)
                        {
                            y--;
                            p++;

                            queue.Add(NewEntry(p, x, y));
                        }

                        for (long i = 0; i < sideLength; i++)
                        {
                            x++;
                            p++;

                            queue.Add(NewEntry(p, x, y));
                        }

                        sideLength++;

                        queue.CompleteAdding();
                    });

                    await Task.WhenAll(producer, consumer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task<Entry> NewEntry(ulong p, long x, long y)
        {
            var entry = new Entry(p, x, y);
            if (p == 1)
            {
                // 1 is a special case
                return entry;
            }

            ulong third = (ulong)(p / 3) + 1;
            long value = third.ToLong();
            using (var cursor = await _entries.FindAsync(new JsonFilterDefinition<Entry>("{ $and : [ { IsPrime : true }, { LongValue : { $gt : " + OneAsLong + " } }, { LongValue : { $lte : " + value + " } } ] }")))
            {
                using (var source = new CancellationTokenSource())
                {
                    var token = source.Token;
                    var isPrime = true;

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

                    entry.IsPrime = isPrime;
                }
            }

            return entry;
        }

        public class Entry
        {
            [BsonId]
            public ObjectId Id { get; set; }

            [BsonIgnore]
            internal ulong Value { get; private set; }

            public Entry()
            {
            }

            public Entry(ulong p, long x, long y, bool isPrime = false)
            {
                Location = new Cordinate()
                {
                    X = x,
                    Y = y
                };
                Value = p;
                IsPrime = isPrime;
            }

            [BsonElement]
            public string StringValue
            {
                get
                {
                    return Value.ToString();
                }
            }

            [BsonElement]
            public long LongValue
            {
                get
                {
                    return Value.ToLong();
                }
                set
                {
                    Value = value.ToULong();
                }
            }

            [BsonElement]
            public Cordinate Location { get; set; }

            [BsonElement]
            [BsonIgnoreIfDefault]
            public bool IsPrime { get; set; }
        }

        public class Cordinate
        {
            public long X { get; set; }
            public long Y { get; set; }
        }
    }
}