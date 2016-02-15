﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    /// <summary>
    /// Prime seive
    /// </summary>
    public class Eratosthenes
    {
        private readonly ConcurrentQueue<long> _primes;

        public Eratosthenes()
        {
            _primes = new ConcurrentQueue<long>();
        }

        public bool IsPrime(long i)
        {
            if (i % 2 == 0)
            {
                return false;
            }
            long half = (long)Math.Sqrt(i);
            object obj1 = new object();
            object obj2 = new object();
            long maxTested = 0;
            bool isComposite = false;
            while (maxTested < half && !isComposite)
            {
                Parallel.ForEach(_primes.Where(p => p >= maxTested), (p, loop) =>
                   {
                       if (isComposite)
                       {
                           loop.Stop();
                       }

                       bool com = i % p == 0;
                       if (com)
                       {
                           lock (obj1)
                           {
                               isComposite |= com;
                               loop.Stop();
                           }
                       }

                       bool con = p > half;
                       lock (obj2)
                       {
                           if (con)
                           {
                               maxTested = long.MaxValue;
                           }
                           else
                           {
                               maxTested = p;
                           }
                       }
                   });
            }

            return maxTested > half && !isComposite;
        }

        /// <summary>
        /// Returns all primes up to the max value.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> Sieve(long max = long.MaxValue)
        {
            yield return 2;
            long i = 3;
            _primes.Enqueue(2);
            while (i <= max)
            {
                if (IsPrime(i))
                {
                    Console.WriteLine(i);
                    _primes.Enqueue(i);
                    yield return i;
                }
                i += 2;
            }
        }
    }
}
