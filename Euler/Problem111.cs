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
        private static char[] digits = new[]{
            '0','1','2','3','4','5','6','7','8','9'
        };
        public override object Solve()
        {
            IDictionary<char, ConcurrentQueue<string>> dictionary = new ConcurrentDictionary<char, ConcurrentQueue<string>>();
            foreach (var key in digits)
            {
                dictionary.Add(key, new ConcurrentQueue<string>());
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
            };

            Parallel.ForEach(seeds, seed =>
            {
                var list = new[] { seed };
                IEnumerable<string> permutations;
                switch (seed[0])
                {
                    case '0':
                    case '2':
                    case '8':
                        permutations = list.SelectMany(s => Permute(s)).SelectMany(s => Permute(s));
                        break;
                    default:
                        permutations = list.SelectMany(s => Permute(s));
                        break;
                }
                //permutations = list.SelectMany(s => Permute(s)).SelectMany(s => Permute(s)).SelectMany(s => Permute(s));
                var hash = new HashSet<string>(permutations);
                FindPrimes(hash, dictionary);
            });

            long sum = 0;
            foreach (var kvp in dictionary.OrderBy(kvp => kvp.Key))
            {
                int m = kvp.Value.Select(p => p.Count(c => c == kvp.Key)).Max();
                var primes = new HashSet<string>(kvp.Value.Where(l => l.Count(c => c == kvp.Key) == m));
                int n = primes.Count;
                var s = primes.Sum(p => long.Parse(p));

                Console.WriteLine("{0}\t{1}\t{2}\t{3}", kvp.Key, m, n, s);
                sum += s;
            }

            return sum;
        }

        private static void FindPrimes(IEnumerable<string> seeds, IDictionary<char, ConcurrentQueue<string>> dictionary)
        {
            foreach (string permutation in seeds)
            {
                long i = long.Parse(permutation);
                if (i % 2 == 0 || i % 3 == 0)
                {
                    continue;
                }
                var duplicates = digits.Where(c => permutation.Count(s => s == c) > 1).ToList();

                if (duplicates.Count == 1)
                {
                    var d = duplicates.FirstOrDefault();
                    var list = dictionary[d];
                    int max = list.Any() ? list.Max(s => s.Count(c => c == d)) : 0;
                    if (permutation.Count(s => s == d) >= max)
                    {
                        if (i % 2 != 0)
                        {
                            object obj = new object();
                            bool prime = true;
                            double m = Math.Sqrt(i) / 2;
                            Parallel.For(1L, (long)m, (p, loop) =>
                            {
                                var modulos = p * 2 + 1;
                                if (i % modulos == 0)
                                {
                                    lock (obj)
                                    {
                                        prime &= false;
                                        loop.Stop();
                                    }
                                }
                            });

                            if (prime)
                            {
                                list.Enqueue(permutation);
                                Console.WriteLine(permutation);
                            }
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
