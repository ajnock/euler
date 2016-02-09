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
        private readonly ConcurrentQueue<long> _primes = new ConcurrentQueue<long>();

        public IEnumerable<long> GetPrimes(long max)
        {
            yield return 2;

            var enumerator = _primes.GetEnumerator();
            while (enumerator.MoveNext() && enumerator.Current <= max)
            {
                yield return enumerator.Current;
            }
        }

        private bool IsPrime(long i)
        {
            if (i % 2 == 0)
            {
                return false;
            }

            foreach (var p in _primes)
            {
                if (p > i / 3)
                {
                    return true;
                }
                else if (i % p == 0)
                {
                    return false;
                }
            }

            return true;
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
            while (i <= max)
            {
                if (IsPrime(i))
                {
                    Console.WriteLine(i + "/" + max);
                    _primes.Enqueue(i);
                    yield return i;
                }
                i += 2;
            }
        }
    }
}
