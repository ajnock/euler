using System.Linq;

namespace Euler
{
    public class Problem134 : Problem
    {
        public override object Solve()
        {
            var e = new Eratosthenes();
            var primes = e.Sieve(1000000).Where(p => p >= 5);
            long sum = 0;
            long q;
            bool first = true;

            foreach (var p in primes)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                }
                q = p;
            }

            return null;
        }

    }
}
