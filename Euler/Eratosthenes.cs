using System;
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
        private readonly List<long> _primes = new List<long>();

        private bool IsPrime(long i)
        {
            if (i % 2 == 0)
            {
                return false;
            }

            foreach (var p in _primes)
            {
                if (p > i / 2)
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
                    //Console.WriteLine(i);
                    _primes.Add(i);
                    yield return i;
                }
                i += 2;
            }
        }
    }
}
