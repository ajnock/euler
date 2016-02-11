using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    public class Problem50 : Problem
    {
        public override object Solve()
        {
            var e = new Eratosthenes();
            var primes = new List<long>();
            var sums = new ConcurrentDictionary<long, long>();

            foreach (var p in e.Seive(1000000))
            {
                primes.Add(p);
                Console.WriteLine(p);
            };

            Parallel.ForEach(primes, p =>
            {
                var sum = p;
                foreach (var q in primes.Where(a => a > p).OrderBy(a => a))
                {
                    var tmp = sum + q;
                    if (primes.Contains(tmp))
                    {
                        sum = tmp;
                    }
                    else
                    {
                        sums.TryAdd(q, sum);
                        break;
                    }
                }
            });

            return sums.Values.Max();
        }
    }
}
