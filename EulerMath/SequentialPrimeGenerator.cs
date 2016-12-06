using System.Collections.Generic;
using System.Linq;

namespace EulerMath
{
    public class SequentialPrimeGenerator : IPrimeGenerator
    {
        public long[] Primes
        {
            get
            {
                return _primes.ToArray();
            }
        }

        public IEnumerable<long> PrimesEnumerable
        {
            get
            {
                return _primes;
            }
        }

        private ICollection<long> _primes = new List<long>();

               /// <summary>
        /// Returns all primes up to the max value.
        /// This implementation is single threaded.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<long> Sieve(long max = long.MaxValue)
        {
            long p = 2;

            while (p <= max)
            {
                _primes.Add(p);

                var temp = p;
                p = 1;
                foreach (var q in _primes)
                {
                    p *= q;
                }
                p++;
                yield return temp;              
            }
        }

        public SequentialPrimeGenerator()
        {
            // TODO: Complete member initialization
        }
    }
}
