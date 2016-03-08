using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (i % 2 == 0)
            {
                return false;
            }
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

        private async void OptimizedProduce(BlockingCollection<long> queue, AutoResetEvent signal, long max, long count)
        {
            await Task.Run(() =>
             {
                 count++;

                 long root = 2L * count + 1L;
                 long limit = Math.Min(root * root, max);
                 long to = (limit + 1L) / 2L;
                 long from = (_maxSieved + 1L) / 2L;

                 Parallel.For(from, to, (k) =>
                 {
                     long p = 2L * k + 1;
                     if (IsPrime(p))
                     {
                         queue.Add(p);
                         signal.Set();
                     }
                 });

                 queue.CompleteAdding();
                 _maxSieved = limit;
                 signal.Set();
             });
        }

        public IEnumerable<long> OptimizedSieve(long max = long.MaxValue)
        {
            // make max odd
            if (max % 2L == 0)
            {
                max--;
            }

            foreach (var p in Sieve(9))
            {
                //NonBlockingConsole.WriteLine(p);
                yield return p;
            }

            var signal = new AutoResetEvent(false);
            long count = 1L;
            while (_maxSieved < max)
            {
                var queue = new BlockingCollection<long>();
                count++;
                OptimizedProduce(queue, signal, max, count);

                while (!queue.IsCompleted && signal.WaitOne() &&
                    (!queue.IsCompleted || queue.Any()))
                {
                    long value;
                    while (queue.TryTake(out value))
                    {
                        //NonBlockingConsole.WriteLine(value);
                        _primes.Add(value);
                        yield return value;
                    }
                }
            }
        }

        public IEnumerable<long> OptimizedSieveSorted(long max = long.MaxValue)
        {
            // make max odd
            if (max % 2L == 0)
            {
                max--;
            }

            foreach (var p in Sieve(9))
            {
                //NonBlockingConsole.WriteLine(p);
                yield return p;
            }

            var signal = new AutoResetEvent(false);
            long count = 1L;
            while (_maxSieved < max)
            {
                var queue = new BlockingCollection<long>();
                count++;
                OptimizedProduce(queue, signal, max, count);

                var sortedSet = new SortedList<long, object>();
                while (!queue.IsCompleted && signal.WaitOne() &&
                    (!queue.IsCompleted || queue.Any()))
                {
                    long value;
                    while (queue.TryTake(out value))
                    {
                        _primes.Add(value);
                        sortedSet.Add(value, null);
                    }
                }

                foreach (var p in sortedSet)
                {
                    yield return p.Key;
                }
            }
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
