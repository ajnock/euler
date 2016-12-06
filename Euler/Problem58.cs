using System;

namespace Euler
{
    class Problem58 : Problem
    {
        public override object Solve()
        {
            long primes = 0;
            for (long k = 1; k <= long.MaxValue; k++)
            {
                var corners = new[] {
                    4L*k*k - 2L*k + 1L,
                    4L*k*k + 2L*k + 1L,
                    4L*k*k + 1L,
                };

                foreach (var p in corners)
                {
                    if (IsPrime(p))
                    {
                        primes++;
                    }
                }

                long total = 4L * k + 1;
                double ratio = primes / (double)total;
                Console.WriteLine(primes + "/" + total + "=" + ratio);
                if (ratio < 0.1)
                {
                    return 2L * k + 1L;
                }
            }

            return null;
        }
    }
}
