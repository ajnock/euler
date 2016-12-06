using System;

namespace Euler
{
    class Problem154 : Problem
    {
        public override object Solve()
        {
            const long n = 200000;
            //long max = 0;
            //long product = 1;
            //while (product < 1e12)
            //{
            //    max++;
            //    product *= max;
            //}

            long total = 0;
            for (long i = 0; i <= n; i++)
            {
                long product = 1;
                if (Factorial(i, ref product))
                {
                    var temp = product;
                    for (long j = 0; j <= n - i; j++)
                    {
                        long k = n - i - j;
                        if (i + j + k == n &&
                            Factorial(j, ref temp) && Factorial(k, ref temp))
                        {
                            NonBlockingConsole.WriteLine(i + " " + j + " " + k);
                            total++;
                        }
                    }
                }
            }

            return total;
        }

        private bool Factorial(long i, ref long product)
        {
            const long max = 1000000000000;

            while (product < max && product > 0 && i > 1)
            {
                try
                {
                    product = checked(product * i);
                }
                catch (OverflowException)
                {
                    return false;
                }
                i--;
            }

            return product < max;
        }
    }
}
