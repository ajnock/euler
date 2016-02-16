using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euler
{
    class Problem58 : Problem
    {
        public override object Solve()
        {
            var e = new Eratosthenes();

            long seed = 3037000499;
            long max = seed * seed;
            List<long> primes = e.Sieve(5).ToList();
            int cornerPrimes = 0;
            long corner = (max - 1L) / 2L;
            Console.WriteLine(corner);

            for (long i = 1L; i < corner; i++)
            {
                long root = 2L * i + 1L;
                long square = root * root;
                long sideLength = root - 1L;
                primes.AddRange(e.Continue(square));

                for (int j = 0; j < 3; j++)
                {
                    if (primes.Contains(square - j * sideLength))
                    {
                        cornerPrimes++;
                    }
                }

                long corners = i * 4L + 1L;
                double ratio = cornerPrimes / (double)corners;
                Console.WriteLine(root + "^2=" + square + ": " + cornerPrimes + "/" + corners + "=" + ratio);
                if (ratio < 0.1)
                {
                    return sideLength;
                }
            }

            return null;
        }
    }
}
