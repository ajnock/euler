using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Euler
{
    class Problem111 : Problem
    {
        private static string[] digits = new[]{
            "0","1","2","3","4","5","6","7","8","9"
        };
        public override object Solve()
        {
            IDictionary<string, List<string>> dictionary = new ConcurrentDictionary<string, List<string>>();
            foreach (var key in digits)
            {
                dictionary.Add(key, new List<string>());
            }

            IEnumerable<string> seeds = new[] {
                "0000000000",
                "1111111111",
                "2222222222",
                "3333333333",
                "4444444444",
                "5555555555",
                "6666666666",
                "7777777777",
                "8888888888",
                "9999999999"
            }.SelectMany(s => Permute(s)).SelectMany(s => Permute(s)).SelectMany(s => Permute(s));

            Parallel.ForEach(seeds, seed =>
            {
                FindPrime(seed, dictionary);
                //var list = new[] { seed };
                //FindPrimes(list.SelectMany(s => Permute(s)).SelectMany(s => Permute(s)).SelectMany(s => Permute(s)), dictionary);
            });

            long sum = 0;
            foreach (var kvp in dictionary)
            {
                int m = kvp.Value.Last().Count(c => c.ToString() == kvp.Key);
                var primes = new HashSet<string>(kvp.Value.Where(l => l.Count(c => c.ToString() == kvp.Key) == m));
                int n = primes.Count;
                var s = primes.Sum(p => long.Parse(p));

                Console.WriteLine("{0}\t{1}\t{2}\t{3}", kvp.Key, m, n, s);
                sum += s;
            }

            return sum;
        }
        
        private static void FindPrime(string permutation, IDictionary<string, List<string>> dictionary)
        {
            long i = long.Parse(permutation);
            if (i % 2 == 0 || i % 3 == 0)
            {
                return;
            }
            var duplicates = digits.Where(c => permutation.Count(s => s.ToString() == c) > 1).ToList();

            if (duplicates.Count == 1)
            {
                var d = duplicates.FirstOrDefault();
                var list = dictionary[d];
                int max = list.Any() ? list.Last().Count(s => s.ToString() == d) : 0;
                if (permutation.Count(s => s.ToString() == d) >= max)
                {
                    bool prime = i % 2 != 0;
                    for (long p = 3; p < i / 3 && prime; p += 2)
                    {
                        prime &= i % p != 0;
                    }
                    if (prime)
                    {
                        list.Add(permutation);
                        Console.WriteLine(i);
                    }
                }
            }
        }

        private static void FindPrimes(IEnumerable<string> seeds, IDictionary<string, List<string>> dictionary)
        {
            var nodes = new HashSet<string>();
            foreach (string permutation in seeds)
            {
                long i = long.Parse(permutation);
                if (i % 2 == 0 || i % 3 == 0)
                {
                    continue;
                }
                var duplicates = digits.Where(c => permutation.Count(s => s.ToString() == c) > 1).ToList();

                if (duplicates.Count == 1)
                {
                    var d = duplicates.FirstOrDefault();
                    var list = dictionary[d];
                    int max = list.Any() ? list.Last().Count(s => s.ToString() == d) : 0;
                    if (permutation.Count(s => s.ToString() == d) >= max)
                    {
                        //bool prime = primes.All(p => i % p != 0);
                        bool prime = i % 2 != 0 && nodes.Add(permutation);
                        for (long p = 3; p < i / 3 && prime; p += 2)
                        {
                            prime &= i % p != 0;
                        }
                        if (prime)
                        {
                            list.Add(permutation);
                            Console.WriteLine(i);
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> Permute(string str)
        {
            for (int a = 0; a < str.Length; a++)
            {
                for (int b = 0; b < 10; b++)
                {
                    var ar = str.ToCharArray();
                    var next = b.ToString().ToCharArray()[0];
                    if (ar[a] != next)
                    {
                        ar[a] = next;

                        var p = string.Join("", ar);
                        if (!p.StartsWith("0"))
                        {
                            yield return p;
                        }
                    }
                }
            }
        }
    }
}
