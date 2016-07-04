using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
