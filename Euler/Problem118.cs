using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Euler
{
    class Problem118 : Problem
    {
        public override object Solve()
        {
            var e = new Eratosthenes();
            //var primes = new List<string>();
            var reg = new Regex(@"(.)(?=\1)");
            var primes = e.OptimizedSieve(100000000)
                .Select(p => p.ToString())
                .Where(s => !s.Contains("0") && !reg.IsMatch(s))
                .ToList();

            Console.WriteLine("=======================" + DateTime.Now);
            var solution = Do(new string[0], primes).Count();

            return solution;
        }



        private IEnumerable<string[]> Do(string[] set, ICollection<string> primes)
        {
            if (string.Join("", set).Length == 9)
            {
                NonBlockingConsole.WriteLine(string.Join(", ", set));
                yield return set;
            }
            else {
                foreach (var p in primes)
                {
                    var nextSet = new string[set.Length + 1];
                    for (int i = 0; i < set.Length; i++)
                    {
                        nextSet[i] = set[i];
                    }

                    nextSet[set.Length] = p;

                    var otherPrimes = primes.Where(s => s.ToCharArray().Any(c => p.ToCharArray().Contains(c))).ToList();
                    var results = Do(nextSet, otherPrimes);
                    foreach (var r in results)
                    {
                        yield return r;
                    }
                }
            }
        }
    }
}
