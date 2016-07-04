using EulerMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    class SequentialPrimeRatio : Problem
    {
        const string columns = "|{0}|\t\t|{1}|\t|{2}|\t|{3}|\t|{4:P}|";
        const string header = "{0}\t{1}\t{2}\t{3}\t{4}";
        public override object Solve()
        {
            var sequentialGenerator = new SequentialPrimeGenerator();
            var erat = new Eratosthenes();
            var fullGenerator = erat.OptimizedSieveSorted().GetEnumerator();

            var sequentials = 0;
            var all = 0;

            Console.WriteLine(header, "Sequential", "Total", "All ", "Total", "Ratio");

            long lastP = 0;
            foreach (var p in sequentialGenerator.Sieve())
            {
                bool more = true;
                sequentials++;
                while (fullGenerator.Current < p)
                {
                    all++;
                    more = fullGenerator.MoveNext();
                }

                if (fullGenerator.Current != p)
                {
                    sequentials--;
                    Console.Write("=");
                }

                double ratio = (double)sequentials / all;
                Console.WriteLine(columns, p, sequentials, fullGenerator.Current, all, ratio);

                if (!more)
                {
                    return new object();
                }

                lastP = p;
            }

            return new object();
        }
    }
}
