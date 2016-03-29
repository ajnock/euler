using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Euler
{
    class Problem154 : Problem
    {
        public override object Solve()
        {
            const long n = 200000;
            long max = 0;
            long product = 1;
            while (product < 1e12)
            {
                max++;
                product *= max;
            }

            long total = 0;
            for (long i = 0; i < 2e5; i++)
            {
                for (long j = 0; j < 2e5; j++)
                {
                    for (long k = 0; k < 2e5; k++)
                    {
                        if (i + j + k == n)
                        {
                            if (TestFactorial(i, j, k))
                            {
                                NonBlockingConsole.WriteLine(i + " " + j + " " + k);
                                total++;
                            }
                        }
                    }
                }
            }

            return max;
        }

        private bool TestFactorial(long i, long j, long k)
        {
            const long max = 1000000000000;
            long product = 1;
            while (product < max && i > 0)
            {
                i--;
                product *= i;
            }
            while (product < max && j > 0)
            {
                j--;
                product *= j;
            }
            while (product < max && k > 0)
            {
                k--;
                product *= k;
            }

            return product < max;
        }
    }
}
