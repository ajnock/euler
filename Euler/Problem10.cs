using System.Linq;

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
