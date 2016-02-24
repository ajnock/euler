using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Euler
{
    class Problem118 : Problem
    {
        public override object Solve()
        {
            var e = new Eratosthenes();
            var primes = new BlockingCollection<string>();
            var buffer = new BlockingCollection<string>();

            //primes.Add("1");
            primes.Add("2");
            Parallel.For(1L, 98765432L / 2L, k =>
            {
                long p = 2L * k + 1L;
                var s = p.ToString();
                if (!s.Contains("0") && s.All(c => s.Count(ch => ch == c) == 1) && IsPrime(p))
                {
                    NonBlockingConsole.WriteLine(s);
                    primes.Add(s);
                }
            });

            var producer = Task.Run(() =>
            {
                Do(new string[0], primes.ToArray(), buffer);

                buffer.CompleteAdding();
            });

            var hash = new HashSet<string>();
            long solution = 0;
            while (!buffer.IsCompleted)
            {
                try
                {
                    var result = buffer.Take();
                    if (hash.Add(result))
                    {
                        solution++;
                        NonBlockingConsole.WriteLine(result);
                    }
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }

            return solution;
        }

        private void Do(string[] set, string[] primes, BlockingCollection<string> buffer)
        {
            if (primes.Any())
            {
                Parallel.ForEach(primes, p =>
               {
                   var nextSet = new string[set.Length + 1];
                   set.CopyTo(nextSet, 0);
                   nextSet[set.Length] = p;

                   var otherPrimes = primes.Where(s => !s.Any(c => p.Contains(c) || set.Any(s2 => s2.Contains(c)))).ToArray();

                   Do(nextSet, otherPrimes, buffer);
               });
            }
            else if (string.Join("", set).Length == 9)
            {
                Array.Sort(set);

                var result = string.Join(" ", set);

                buffer.Add(result);
            }
        }
    }
}
