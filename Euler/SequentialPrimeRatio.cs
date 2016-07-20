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
        const string columns = "{0}  {1}  {2}  {3}  {4:P}  {5}";
        const string header = "|{0}|\t|{1}|\t|{2}|\t|{3}|\t|{4}|\t|{5}|";
        public override object Solve()
        {
            var sequentialGenerator = new SequentialPrimeGenerator();
            var erat = new Eratosthenes();
            var fullGenerator = erat.OptimizedSieveSorted().GetEnumerator();

            var sequentials = 0;
            var all = 0;

            Console.WriteLine(header, "Sequential", "Total", "All", "Total", "Ratio", "Primality");

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

                string format;
                if (fullGenerator.Current != p)
                {
                    sequentials--;
                    format = "composite";
                }
                else
                {
                    format = "prime";
                }

                double ratio = (double)sequentials / all;
                Console.WriteLine(columns, p, sequentials, fullGenerator.Current, all, ratio, format);

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
