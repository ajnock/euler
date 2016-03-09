using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EulerMath
{
   public class Fibonacci
    {
        public static bool IsFibonacci(long n)
        {
            long a = 5L * n * n + 4L;
            long b = 5L * n * n - 4L;

            return IsPerfectSquare(a) || IsPerfectSquare(b);
        }

        private static bool IsPerfectSquare(long n)
        {
            var root =(long) Math.Round(Math.Sqrt(n));
            return root * root == n;
        }
    }
}
