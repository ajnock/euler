using Euler;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ulam
{
    public class MongoSpiral : ISpiral
    {
        public class Entry
        {
            private readonly ulong _p;
            public Entry(ulong p, long x, long y)
            {
                Location = new Cordinate()
                {
                    X = x,
                    Y = y
                };
                _p = p;
                StringValue = p.ToString();
                LongValue = ToLong(p);
            }

            public string StringValue { get; set; }
            public long LongValue { get; set; }

            public Cordinate Location { get; set; }
        }

        public class Cordinate
        {
            public long X { get; set; }
            public long Y { get; set; }

        }
        private readonly ulong _max;

        public MongoSpiral(ulong max)
        {
            _max = max;
        }

        private static long ToLong(ulong u)
        {
            ulong l = u - (ulong)long.MaxValue;
            return (long)l;
        }

        private Entry WriteJson(ulong p, long x, long y)
        {
            return new Entry(p, x, y);
        }

        public async Task GenerateAndSave(string file)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("Ulam").GetCollection<Entry>("map");

            using (var fileStream = File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(fileStream))
            {
                try
                {
                    ulong p = 1;
                    long x = 0;
                    long y = 0;
                    long sideLength = 0;

                    // 1 is special
                    var doc = WriteJson(p, x, y);
                    await db.InsertOneAsync(doc);

                    while (p <= _max)
                    {
                        sideLength++;
                        for (long i = 0; i < sideLength; i++)
                        {
                            x++;
                            p++;

                            doc = WriteJson(p, x, y);
                            await db.InsertOneAsync(doc);
                        }

                        for (long i = 0; i < sideLength; i++)
                        {
                            y++;
                            p++;

                            doc = WriteJson(p, x, y);
                            await db.InsertOneAsync(doc);
                        }

                        sideLength++;
                        for (long i = 0; i < sideLength; i++)
                        {
                            // print the even square
                            if (i == sideLength - 1)
                            {
                                NonBlockingConsole.WriteLine(p);
                            }

                            x--;
                            p++;

                            doc = WriteJson(p, x, y);
                            await db.InsertOneAsync(doc);
                        }

                        for (long i = 0; i < sideLength; i++)
                        {
                            y--;
                            p++;

                            doc = WriteJson(p, x, y);
                            await db.InsertOneAsync(doc);
                        }
                    }
                }
                catch (OverflowException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}