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

        private bool IsPrime(long i)
        {
            if (i % 2 == 0)
            {
                return false;
            }
            long half = (long)Math.Sqrt(i);
            object obj1 = new object();
            object obj2 = new object();
            bool testIsConclusive = false;
            bool isComposite = false;
            while (!testIsConclusive && !isComposite)
            {
                Parallel.ForEach(_primes, (p, loopState) =>
                {
                    bool com = i % p == 0;
                    if (com)
                    {
                        lock(obj1)
                        {
                            isComposite |= com;
                            loopState.Stop();
                        }
                    }

                    Task.Run(() => {
                        bool con = p > half;
                        if (con)
                        {
                            lock (obj2)
                            {
                                testIsConclusive |= con;
                            }
                        }
                    });
                });
            }

            return testIsConclusive && !isComposite;
        }

        /// <summary>
        /// Returns all primes up to the max value.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> Seive(long max = long.MaxValue)
        {
            yield return 2;

            long i = 3;
            _primes.Enqueue(3);
            while (i <= max)
            {
                if (IsPrime(i))
                {
                    //Console.WriteLine(i);
                    _primes.Enqueue(i);
                    yield return i;
                }
                i += 2;
            }
        }
    }
}
