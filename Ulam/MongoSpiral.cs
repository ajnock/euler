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

namespace Ulam
{
    public class MongoSpiral : ISpiral
    {

        private readonly ulong _max;
        private readonly IMongoCollection<Entry> _entries;

        public MongoSpiral(ulong max)
        {
            _max = max;
            var client = new MongoClient();
            _entries = client.GetDatabase("Ulam").GetCollection<Entry>("map");

        }

        public async Task GenerateAndSave(string file)
        {
            try
            {
                ulong p = 1;
                long x = 0;
                long y = 0;
                long sideLength = 0;

                var sort = new JsonSortDefinition<Entry>("{ \"Location.Y\" : 1, \"Location.X\" : 1 }");
                var cursor = await _entries.FindAsync(FilterDefinition<Entry>.Empty, new FindOptions<Entry, Entry>()
                {
                    Sort = sort,
                    Limit = 1
                });
                var cornerDoc = await cursor.FirstOrDefaultAsync();

                if (cornerDoc == null)
                {
                    // 1 is special
                    var doc = new Entry(p, x, y);
                    await _entries.InsertOneAsync(doc);
                }
                else
                {
                    if (cornerDoc.Value >= _max)
                    {
                        Console.WriteLine("Nothing to generate");
                        return;
                    }

                    p = cornerDoc.Value;
                    x = y = (long)Math.Min(cornerDoc.Location.X, cornerDoc.Location.Y);

                    // calculate the current side length
                    long root = (long)Math.Floor(Math.Sqrt(p) + 1);
                    sideLength = -2 * x;

                    // remove excess to get ourselves to an easy starting point so we can jump right into the algorithm
                    var result = _entries.DeleteMany(new JsonFilterDefinition<Entry>("{ LongValue : { $gt : " + cornerDoc.LongValue + " } }"));
                    Console.WriteLine("Deleted " + result.DeletedCount + " documents");
                }

                while (p <= _max)
                {
                    sideLength++;
                    for (long i = 0; i < sideLength; i++)
                    {
                        x++;
                        p++;

                        var doc = await NewEntry(p, x, y);
                        await _entries.InsertOneAsync(doc);
                    }

                    for (long i = 0; i < sideLength; i++)
                    {
                        y++;
                        p++;

                        var doc = await NewEntry(p, x, y);
                        await _entries.InsertOneAsync(doc);
                    }

                    sideLength++;
                    for (long i = 0; i < sideLength; i++)
                    {
                        // print the even square
                        //if (i == sideLength - 1)
                        //{
                        //    NonBlockingConsole.WriteLine(p);
                        //}

                        x--;
                        p++;

                        var doc = await NewEntry(p, x, y);
                        await _entries.InsertOneAsync(doc);
                    }

                    for (long i = 0; i < sideLength; i++)
                    {
                        y--;
                        p++;

                        var doc = await NewEntry(p, x, y);
                        await _entries.InsertOneAsync(doc);
                    }
                }
            }
            catch (OverflowException ex)
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
            var cursor = await _entries.FindAsync(new JsonFilterDefinition<Entry>("{ $and : [ { IsPrime : true }, { LongValue : { $lte : " + value + " } } ] }"));

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

                if (isPrime)
                {
                    NonBlockingConsole.WriteLine(p);
                }
            }

            return entry;
        }

        public class Entry
        {
            public ObjectId _id { get; set; }

            internal ulong Value { get; private set; }

            public Entry()
            {
            }
            public Entry(ulong p, long x, long y)
            {
                Location = new Cordinate()
                {
                    X = x,
                    Y = y
                };
                Value = p;
                StringValue = p.ToString();
                _longValue = p.ToLong();
                IsPrime = false;
            }

            public string StringValue { get; set; }

            private long _longValue;
            public long LongValue
            {
                get
                {
                    return _longValue;
                }
                set
                {
                    _longValue = value;
                    Value = value.ToULong();
                }
            }

            public Cordinate Location { get; set; }

            public bool IsPrime { get; set; }
        }

        public class Cordinate
        {
            public long X { get; set; }
            public long Y { get; set; }
        }
    }
}