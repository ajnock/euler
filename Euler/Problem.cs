using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    public abstract class Problem
    {
        public abstract object Solve();

        public static bool IsPrime(long p)
        {
            if (p % 2L == 0)
            {
                return false;
            }

            bool prime = true;
            Parallel.For(1L, (long)(Math.Sqrt(p) - 1L) / 2L, (k, loop) =>
             {
                 if (p % (2L * k + 1L) == 0L)
                 {
                     prime = false;
                     loop.Stop();
                 }
             });

            return prime;
        }
    }
}
