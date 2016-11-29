using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EulerMath
{
    public class Fibonacci
    {
        /// <summary>
        /// Returns true if <paramref name="n"/> is part of the Fibonacci number.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsFibonacci(long n)
        {
            long a = 5L * n * n + 4L;
            long b = 5L * n * n - 4L;

            return IsPerfectSquare(a) || IsPerfectSquare(b);
        }

        /// <summary>
        /// Returns true if <paramref name="n"/> is a perfect square
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static bool IsPerfectSquare(long n)
        {
            var root = (long)Math.Round(Math.Sqrt(n));
            return root * root == n;
        }

        public static IEnumerable<long> GetFibonaccis(long max)
        {
            long f0 = 0;
            long f1 = 1;
            long f2 = f0 + f1;

            while (f0 + f1 <= max)
            {
                f2 = f0 + f1;
                f0 = f1;
                f1 = f2;
                yield return f2;
            }
        }

        public static bool IsFibonacci(string p)
        {
            return IsFibonacci(long.Parse(p));
        }
    }
}
