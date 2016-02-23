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
            var hash = new HashSet<string>();

            primes.Add("1");
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
                   for (int i = 0; i < set.Length; i++)
                   {
                       nextSet[i] = set[i];
                   }
                   nextSet[set.Length] = p;

                   var usedDigits = p + string.Join("", set);
                   var otherPrimes = primes.Where(s => !s.Any(c => usedDigits.Contains(c))).ToArray();

                   Do(nextSet, otherPrimes, buffer);
               });
            }
            else if (string.Join("", set).Length == 9)
            {
                Array.Sort(set);

                var result = string.Join(" ", set);

                //NonBlockingConsole.WriteLine(result);
                buffer.Add(result);
            }
        }
    }
}
