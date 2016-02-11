using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    class Problem111 : Problem
    {
        public override object Solve()
        {
            return Solve(10);
        }

        private static char[] digits = new[]{
            '0','1','2','3','4','5','6','7','8','9'
        };

        public object Solve(int n)
        {
            IDictionary<char, ConcurrentQueue<string>> dictionary = new ConcurrentDictionary<char, ConcurrentQueue<string>>();
            foreach (var key in digits)
            {
                dictionary.Add(key, new ConcurrentQueue<string>());
            }

            Parallel.ForEach(digits, digit =>
            {
                var builder = new StringBuilder();
                builder.Append(digit, n);
                string seed = builder.ToString();

                IEnumerable<string> permutations = Permute(seed);

                while (dictionary[seed[0]].Count == 0)
                {
                    var hash = new HashSet<string>(permutations);

                    Parallel.ForEach(hash, permutation =>
                    {
                        var duplicates = digits.Where(c => permutation.Count(s => s == c) > 1).ToList();

                        foreach (var d in duplicates)
                        {
                            var list = dictionary[d];
                            int max = list.Any() ? list.Max(s => s.Count(c => c == d)) : 0;
                            if (permutation.Count(s => s == d) >= max && IsPrime(long.Parse(permutation)))
                            {
                                list.Enqueue(permutation);
                                Console.WriteLine(permutation);
                            }
                        }
                    });

                    permutations = hash.SelectMany(s => Permute(s));
                }
            });

            Console.WriteLine();
            long sum = 0;
            foreach (var kvp in dictionary.OrderBy(kvp => kvp.Key))
            {
                int M = kvp.Value.Select(p => p.Count(c => c == kvp.Key)).Max();
                var primes = kvp.Value
                    .Where(l => l.Count(c => c == kvp.Key) == M)
                    .Select(l => long.Parse(l))
                    .OrderBy(l => l)
                    .ToList();
                int N = primes.Count;
                long S = primes.Sum();

                Console.WriteLine("{0}\t{1}\t{2}\t{3}", kvp.Key, M, N, S);
                //Console.WriteLine(string.Join(" ", primes));
                sum += S;
            }

            return sum;
        }

        private static bool IsPrime(long i)
        {
            if (i % 2 == 0)
            {
                return false;
            }

            object obj = new object();
            bool prime = true;
            double m = Math.Sqrt(i) / 2 + 1;
            Parallel.For(1L, (long)m, (p, loop) =>
            {
                long modulos = p * 2L + 1L;
                if (i % modulos == 0L)
                {
                    lock (obj)
                    {
                        prime &= false;
                        loop.Stop();
                    }
                }
            });

            return prime;
        }

        private static IEnumerable<string> Permute(string seed)
        {
            for (int i = 0; i < seed.Length; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var chars = seed.ToCharArray();
                    var next = j.ToString().ToCharArray()[0];
                    chars[i] = next;
                    var permutation = string.Join("", chars);

                    if (!permutation.StartsWith("0"))
                    {
                        yield return permutation;
                    }
                }
            }
        }
    }
}
