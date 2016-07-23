using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    /// <summary>
    /// The Fibonacci sequence is defined by the recurrence relation:
    /// 
    /// Fn = Fn−1 + Fn−2, where F1 = 1 and F2 = 1.
    /// It turns out that F541, which contains 113 digits, is the first Fibonacci number for which the last nine digits are 1-9 pandigital 
    /// (contain all the digits 1 to 9, but not necessarily in order). And F2749, which contains 575 digits, is the first Fibonacci number for 
    /// which the first nine digits are 1-9 pandigital.
    /// 
    /// Given that Fk is the first Fibonacci number for which the first nine digits AND the last nine digits are 1-9 pandigital, find k.
    /// </summary>
    public class Problem104 : Problem
    {
        public override object Solve()
        {
            var set = new HashSet<long>();
            long i = 1;
            long j = 1;

            long n = 2;
            while (n <= 15e8 + 1000)
            {
                //bool left = IsPandigitialLeft(j);
                //bool right = IsPandigitialRight(j);

                //if (left)
                //{
                //    Console.WriteLine("{0} {1}", n, j);
                //}
                //if (right)
                //{
                //    Console.WriteLine("{0} {1}", n, j);
                //}
                //if (right && left && j.ToString().Length >= 20)
                //{
                //    return n;
                //}

                if (IsPandigitialRight(j))
                {
                    var shrt = j % tenDigits;
                    set.Add(shrt);
                    Console.WriteLine("{0} {1}", shrt, n);
                }

                long tmp = j;
                j = (j + i) % tenDigits;
                i = tmp;
                n++;
            }

            return string.Format("{0} {1}", set.Count(), set.Max());
        }
    }
}
