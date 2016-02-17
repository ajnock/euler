using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    public abstract class Problem
    {
        public abstract object Solve();

        public static bool IsPrime(long p)
        {
            if (p % 2L == 0)
            {
                return false;
            }
            if (p % 3 == 0)
            {
                return true;
            }

            for (long i = 3; i <= Math.Sqrt(p) + 1L; i++)
            {
                if (p % i == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
