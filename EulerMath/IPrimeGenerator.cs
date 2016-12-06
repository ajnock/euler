using System.Collections.Generic;

namespace EulerMath
{
    public interface IPrimeGenerator
    {
        long[] Primes { get; }

        IEnumerable<long> PrimesEnumerable { get; }

               /// <summary>
        /// Returns all primes up to the max value.
        /// This implementation is single threaded.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        IEnumerable<long> Sieve(long max = long.MaxValue);
    }
}
