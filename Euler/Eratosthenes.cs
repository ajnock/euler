using System;
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
        private readonly List<long> _primes;

        public Eratosthenes()
        {
            _primes = new List<long>();
        }

        public bool IsPrime(long i)
        {
            object obj = new object();
            bool isComposite = false;

            Parallel.ForEach(_primes, (p, loop) =>
            {
                if (isComposite)
                {
                    loop.Stop();
                }

                if (i % p == 0)
                {
                    lock (obj)
                    {
                        isComposite |= true;
                        loop.Stop();
                    }
                }
            });

            return !isComposite;
        }

        long _maxSieved;

        /// <summary>
        /// Returns all primes up to the max value.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> Sieve(long max = long.MaxValue)
        {
            yield return 2;
            long i = 3;
            _primes.Add(2);
            while (i <= max)
            {
                if (IsPrime(i))
                {
                    //Console.WriteLine(i);
                    _primes.Add(i);
                    yield return i;
                }
                i += 2;
            }

            _maxSieved = max;
        }

        public IEnumerable<long> Continue(long max = long.MaxValue)
        {
            var queue = new ConcurrentQueue<long>();
            long min = (_maxSieved - 1L) / 2L;
            long limit = (max - 1L) / 2L;
            Parallel.For(min, limit, (k) =>
            {
                long i = 2L * k + 1;
                if (IsPrime(i))
                {
                    //Console.WriteLine(i);
                    queue.Enqueue(i);
                }
            });

            foreach (var p in queue)
            {
                _primes.Add(p);
                yield return p;
            }
        }

        public long[] Primes
        {
            get { return _primes.ToArray(); ; }
        }
    }
}
