using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    class Problem10 : Problem
    {
        public override object Solve()
        {
            var e = new Eratosthenes();
            return e.Sieve(2000000).Sum();
        }
    }
}
