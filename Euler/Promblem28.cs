using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ulam;

namespace Euler
{
    public class Promblem28 : Problem
    {
        public override object Solve()
        {
            long sum = 1;

            for (int i = 3; i <= 1001; i += 2)
            {
                sum += i * i;
                sum += i * i - (i - 1);
                sum += i * i - 2 * (i - 1);
                sum += i * i - 3 * (i - 1);
            }

            return sum;
        }
    }
}
