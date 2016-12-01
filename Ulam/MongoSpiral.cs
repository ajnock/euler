using Euler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ulam
{
    class MongoSpiral : ISpiral
    {
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

        private string WriteJson(ulong p, long x, long y)
        {
            var l = ToLong(p);
            return string.Format(@"{{ StringValue : ""{0}"", LongValue : NumberLong({3}), Location : {{ X : NumberLong({1}), Y : NumberLong({2}) }} }}", p, x, y, l);
        }

        public async Task GenerateAndSave(string file)
        {
            using (var fileStream = File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(fileStream))
            {
                try
                {
                    ulong p = 1;
                    long x = 0;
                    long y = 0;
                    long sideLength = 0;

                    //NonBlockingConsole.WriteLine(p);
                    var doc = WriteJson(p, x, y);
                    await writer.WriteLineAsync(doc);

                    while (p <= _max)
                    {
                        sideLength++;
                        for (long i = 0; i < sideLength; i++)
                        {
                            x++;
                            p++;

                            //NonBlockingConsole.WriteLine(p);
                            doc = WriteJson(p, x, y);
                            await writer.WriteLineAsync(doc);
                        }

                        for (long i = 0; i < sideLength; i++)
                        {
                            y++;
                            p++;

                            //NonBlockingConsole.WriteLine(p);
                            doc = WriteJson(p, x, y);
                            await writer.WriteLineAsync(doc);
                        }

                        sideLength++;
                        for (long i = 0; i < sideLength; i++)
                        {
                            x--;
                            p++;

                            //NonBlockingConsole.WriteLine(p);
                            doc = WriteJson(p, x, y);
                            await writer.WriteLineAsync(doc);

                        }
                        for (long i = 0; i < sideLength; i++)
                        {
                            y--;
                            p++;

                            //NonBlockingConsole.WriteLine(p);
                            doc = WriteJson(p, x, y);
                            await writer.WriteLineAsync(doc);
                        }

                        NonBlockingConsole.WriteLine(p);
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