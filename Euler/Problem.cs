using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euler
{
    public abstract class Problem
    {
        protected const long tenDigits = 1000000000;

        public abstract object Solve();

        public static bool IsPrime(long p)
        {
            if (p % 2L == 0)
            {
                return false;
            }

            bool prime = true;
            Parallel.For(1L, (long)Math.Sqrt(p) / 2L + 1L, (k, loop) =>
                {
                    if (p % (2L * k + 1L) == 0L)
                    {
                        prime = false;
                        loop.Stop();
                    }
                });

            return prime;
        }

        public static bool IsPandigital(string s)
        {
            if (s.Length != 10)
            {
                return false;
            }

            for (int i = 1; i <= 9; i++)
            {
                if (!s.Contains(i.ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsPandigitialTen(long p)
        {
            string str = p.ToString();

            return IsPandigital(str);
        }

        public static bool IsPandigitialLeft(long p)
        {
            if (p < tenDigits)
            {
                return false;
            }

            string str = p.ToString().Substring(0, 10);

            return IsPandigital(str);
        }

        public static bool IsPandigitialRight(long p)
        {
            var leastSignificant = p % tenDigits;
            string str = leastSignificant.ToString();

            return IsPandigital(str);
        }

        public static bool IsPandigitialLeft(ulong p)
        {
            if (p < tenDigits)
            {
                return false;
            }

            string str = p.ToString().Substring(0, 10);

            return IsPandigital(str);
        }

        public static bool IsPandigitialRight(ulong p)
        {
            var leastSignificant = p % tenDigits;
            string str = leastSignificant.ToString();

            return IsPandigital(str);
        }

        public static IEnumerable<long> GetRandomPandigitials()
        {
            var queue = new BlockingCollection<long>();
            Parallel.For(123456789, tenDigits, new ParallelOptions(), (p) =>
            {
                if (IsPandigital(p.ToString()))
                {
                    queue.Add(p);
                }
            });

            return queue.GetConsumingEnumerable();
        }

        public static IEnumerable<string> GetPandigitials()
        {
            return Recurse(string.Empty, "987654321");
        }

        private static IEnumerable<string> Recurse(string number, string whatsLeft)
        {
            if (whatsLeft == "")
            {
                yield return number;
            }
            else
            {
                for (int i = 0; i < whatsLeft.Length; i++)
                {
                    number += whatsLeft[i];
                    whatsLeft = whatsLeft.Remove(i, 1);
                    var set = Recurse(number, whatsLeft);
                    foreach (var solution in set)
                    {
                        yield return solution;
                    }
                }
            }
        }
    }
}
