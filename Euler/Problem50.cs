using System;
using System.Linq;
using System.Threading.Tasks;

namespace Euler
{
    public class Problem50 : Problem
    {
        volatile int maxCount = 0;

        public override object Solve()
        {
            object obj = new object();
            var e = new Eratosthenes();
            long max = 1000000;
            var primes = e.Sieve(max).ToList();
            object solution = null;

            Parallel.ForEach(primes.Where(p => p < max / 2), p =>
                   {
                       long sum = p;
                       int count = 1;
                       string work = p.ToString();
                       foreach (var q in primes.Where(q => q > p).OrderBy(q => q))
                       {
                           if (sum > max)
                           {
                               break;
                           }

                           count++;
                           sum += q;
                           work += " + " + q;

                           if (primes.Contains(sum))
                           {
                               if (count > maxCount)
                               {
                                   lock (obj)
                                   {
                                       if (count > maxCount)
                                       {
                                           Console.WriteLine(count + " => " + sum);

                                           maxCount = count;
                                           solution = work + " = " + sum;
                                       }
                                   }
                               }
                           }
                       }
                   });

            return solution;
        }
    }
}
