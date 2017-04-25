using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EulerMath;

namespace Euler
{
    class Problem182 : Problem
    {
        public override object Solve()
        {
            long p = 1009;
            long q = 3646;
            long n = p * q;
            long phi = (p - 1) * (q - 1);

            var primes = new Eratosthenes();
            return primes.Sieve(phi).Sum();
            ////long min = long.MaxValue;
            //long sum = 0;
            ////var map = new ConcurrentDictionary<long, long>();

            //for (long e = 2; e < phi; e++)
            ////Parallel.For(2, phi, e =>
            //{
            //    if (e % 2 == 0 || e % 3 == 0 || e % 5 == 0 || e % 61 == 0)
            //    {
            //        continue;
            //    }

            //    //long count = 0;
            //    //for (long m = 0; m < n; m++)
            //    //{
            //    //    long c = (long)Math.Pow(m, e) % n;
            //    //    if (m == c)
            //    //    {
            //    //        count++;
            //    //    }
            //    //}

            //    //if (count > 2)
            //    //{
            //    //    NonBlockingConsole.WriteLine(e + " => " + count);
            //    //}
            //    sum += e;
            //}

            //return sum;
        }
    }
}
