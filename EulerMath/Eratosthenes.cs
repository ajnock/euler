using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EulerMath;

namespace Euler
{
    /// <summary>
    /// Prime seive
    /// </summary>
    public class Eratosthenes : IPrimeGenerator
    {
        /// <summary>
        /// All primes found so far
        /// </summary>
        private readonly BlockingCollection<long> _primes;

        long _maxSieved;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public Eratosthenes()
        {
            _primes = new BlockingCollection<long>();
        }

        public IEnumerable<long> PrimesEnumerable
        {
            get
            {
                return _primes;
            }
        }

        private class Head
        {
            public long Prime;
            public long Multiple;

            public Head(long p)
            {
                Prime = p;
                Multiple = p;
            }

            public void Increment()
            {
                Multiple += Prime;
            }
        }

        /// <summary>
        /// Multithreaded hydra approach
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> Sieve2(long max = long.MaxValue)
        {
            var heads = new List<Head>();
            heads.Add(new Head(2));
            long pointer = 2;
            _maxSieved = 2;
            yield return 2;

            while (_maxSieved < max)
            {
                pointer++;
                _maxSieved = pointer;

                if (heads.All(p => p.Prime != pointer))
                {
                    heads.Add(new Head(pointer));
                    yield return pointer;
                }

                foreach (var head in heads)
                {
                    if (head.Prime <= pointer)
                    {
                        head.Increment();
                    }
                }
            }
        }

        public bool IsPrime(long i)
        {
            bool isPrime = true;
            long max = 1L + (long)Math.Sqrt(i);

            Parallel.ForEach(_primes.Where(p => p <= max), (p, loop) =>
               {
                   if (i % p == 0)
                   {
                       isPrime = false;
                       loop.Stop();
                   }
               });

            return isPrime;
        }

        /// <summary>
        /// Returns all primes up to the max value.
        /// This implementation is single threaded.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> Sieve(long max = long.MaxValue)
        {
            yield return 2;
            _primes.Add(2);

            long i = 3;
            while (i <= max)
            {
                if (IsPrime(i))
                {
                    _primes.Add(i);
                    yield return i;
                }
                i += 2;
            }

            _maxSieved = max;
        }

        private async Task Produce(BlockingCollection<long> queue, long max, long k)
        {
            long root = 2L * k + 1L;
            long limit = Math.Min(root * root, max);
            long to = (limit + 1L) / 2L;
            long from = (_maxSieved + 1L) / 2L;

            Parallel.For(from, to, i =>
            {
                long p = 2L * i + 1L;
                if (IsPrime(p))
                {
                    queue.Add(p);
                }
            });

            _maxSieved = limit;
            queue.CompleteAdding();
        }

        /// <summary>
        /// Optimized method that does not return primes in order.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> OptimizedSieve(long max = long.MaxValue)
        {
            // make max odd
            if (max % 2L == 0)
            {
                max--;
            }

            foreach (var p in Sieve(9))
            {
                yield return p;
            }

            long k = 1L;
            Task producer = null;

            while (_maxSieved < max)
            {
                k++;
                var queue = new BlockingCollection<long>();
                using (producer = Produce(queue, max, k))
                {
                    foreach (var prime in queue.GetConsumingEnumerable())
                    {
                        _primes.Add(prime);
                        yield return prime;
                    }
                }
            }
        }

        /// <summary>
        /// Optimized method that returns primes in ascending order.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> OptimizedSieveSorted(long max = long.MaxValue)
        {
            // make max odd
            if (max % 2L == 0)
            {
                max--;
            }

            foreach (var p in Sieve(9))
            {
                yield return p;
            }

            long k = 1L;
            while (_maxSieved < max)
            {
                var queue = new BlockingCollection<long>();
                k++;
                using (var producer = Produce(queue, max, k))
                {
                    var sortedSet = new SortedList<long, object>();
                    foreach (var prime in queue.GetConsumingEnumerable())
                    {
                        sortedSet.Add(prime, null);
                    }

                    foreach (var p in sortedSet)
                    {
                        var prime = p.Key;
                        _primes.Add(prime);
                        yield return prime;
                    }
                }
            }
        }

        /// <summary>
        /// Uses a file to pickup where we left off
        /// </summary>
        /// <param name="max"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public IEnumerable<long> CachedSieve(long max = long.MaxValue, string file = @"D:\primes100000000.txt")
        {
            long l = 0;
            using (var reader = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    l = long.Parse(line);
                    _primes.Add(l);
                    yield return l;
                }
            }

            foreach (var p in Continue(max))
            {
                yield return p;
            }
        }

        public IEnumerable<long> Continue(long max = long.MaxValue)
        {
            var queue = new BlockingCollection<long>();
            var producer = Task.Run(() =>
            {
                long min = (_maxSieved - 1L) / 2L;
                long limit = (max - 1L) / 2L;
                Parallel.For(min, limit, (k) =>
                {
                    long i = 2L * k + 1;
                    if (IsPrime(i))
                    {
                        queue.Add(i);
                    }
                });

                queue.CompleteAdding();
            });

            while (!queue.IsCompleted || queue.Any())
            {
                long value;
                if (queue.TryTake(out value))
                {
                    yield return value;
                }
            }
            _maxSieved = max;
        }

        /// <summary>
        /// Returns an array of all primes sieved so far.
        /// </summary>
        public long[] Primes
        {
            get { return _primes.ToArray(); ; }
        }
    }
}
