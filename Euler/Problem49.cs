using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    public class Problem49 : Problem
    {
        public override object Solve()
        {
            var e = new Eratosthenes();
            var primes = e.Seive(10000).Where(p => p > 999 && p < 10000 && !p.ToString().Contains("0"));
            var dictionary = new Dictionary<string, List<long>>();
            foreach (var p in primes)
            {
                var ar = p.ToString().ToCharArray().Select(c => c.ToString()).ToList();
                var hash = string.Join("", ar.Select(i => int.Parse(i)).OrderBy(c => c));
                List<long> set;
                if (!dictionary.TryGetValue(hash, out set))
                {
                    set = new List<long>();
                    dictionary.Add(hash, set);
                }
                set.Add(p);
            }

            foreach (var kvp in dictionary)
            {
                var set = kvp.Value.OrderBy(i => i).ToList();
                var hash = kvp.Key;

                if (hash == "1478")
                {
                    Console.WriteLine(hash + " => " + string.Join("", set));
                }
                else if (set.Count >= 3)
                {
                    for (int i = 0; i < set.Count; i++)
                    {
                        for (int j = i + 1; j < set.Count; j++)
                        {
                            for (int k = j + 1; k < set.Count; k++)
                            {
                                if (set[k] - set[j] == set[j] - set[i])
                                {
                                    return hash + " => " + string.Concat(set[i], set[j], set[k]);
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
