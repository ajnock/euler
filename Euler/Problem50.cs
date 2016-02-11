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
            var solutionsDictionary = new ConcurrentDictionary<int, long>();
            var max = 1000000;
            foreach (var p in e.Sieve(max))
            {
                primes.Add(p);
                Console.WriteLine(p);
            };

            Parallel.ForEach(primes, p =>
            {
                int count = 1;
                long sum = p;
                foreach (var q in primes.Where(q => q > p).OrderBy(q => q))
                {
                    sum += q;
                    count++;

                    if (sum > max)
                    {
                        break;
                    }

                    if (primes.Contains(sum))
                    {
                        solutionsDictionary.AddOrUpdate(count, sum, (k, v) => sum);
                    }
                }
            });

            var solution = solutionsDictionary.OrderBy(kvp => kvp.Key).Last();
            return solution.Key + " => " + solution.Value;
        }
    }
}
