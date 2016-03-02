using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            // make max odd
            max -= max % 2L;

            yield return 2;
            _primes.Add(2);

            long i = 3;
            while (i <= max)
            {
                if (IsPrime(i))
                {
                    //NonBlockingConsole.WriteLine(i);
                    _primes.Add(i);
                    yield return i;
                }
                i += 2;
            }

            _maxSieved = max;
        }

        public IEnumerable<long> OptimizedSieve(long max = long.MaxValue)
        {
            // make max odd
            max -= max % 2L;

            _primes.Add(2);
            _maxSieved = 2;
            yield return 2;
            //NonBlockingConsole.WriteLine(2);

            _primes.Add(3);
            _maxSieved = 3;
            yield return 3;
            //NonBlockingConsole.WriteLine(3);

            var buffer = new BlockingCollection<long>();
            var reset = new ManualResetEvent(true);

            var producer = Task.Run(() =>
            {
                while (_maxSieved < max)
                {
                    long page = Math.Min(_maxSieved * _maxSieved, max);
                    Produce(buffer, page, reset);
                }

                reset.Set();
                buffer.CompleteAdding();
            });

            while (!buffer.IsCompleted || (buffer.Any() && reset.Set()))
            {
                long prime;
                while (reset.WaitOne() && buffer.TryTake(out prime))
                {
                    _primes.Add(prime);
                    //NonBlockingConsole.WriteLine(prime);

                    yield return prime;
                }

                if (!buffer.Any())
                {
                    reset.Reset();
                }
            }
        }

        private void Produce(BlockingCollection<long> buffer, long max, ManualResetEvent alert)
        {
            long page = Math.Min(_maxSieved * _maxSieved, max);
            long min = (_maxSieved - 1L) / 2L;
            long limit = (page - 1L) / 2L;
            Parallel.For(min, limit, (k) =>
            {
                long i = 2L * k + 1;
                if (IsPrime(i))
                {
                    buffer.Add(i);
                    alert.Set();
                }
            });

            _maxSieved = max;
        }

        public IEnumerable<long> CachedSieve(long max = long.MaxValue, string file = @"D:\primes.txt")
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

        public long[] Primes
        {
            get { return _primes.ToArray(); ; }
        }
    }
}
