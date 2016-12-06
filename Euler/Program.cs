using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Euler
{
    class Program
    {
        private static readonly IDictionary<int, Problem> Problems = new Dictionary<int, Problem>
        {
            { 10, new Problem10() },
            { 41, new Problem41() },
            { 96, new Problem96() },
            { 49, new Problem49() },
            { 111, new Problem111() },
            { 50, new Problem50() },
            { 58, new Problem58() },
            { 134, new Problem134() },
            { 118, new Problem118() },
            { 28, new Promblem28() },
            { 154, new Problem154() },
            { 0, new SequentialPrimeRatio() },
            { 104, new Problem104() }
        };
        static int Main(string[] args)
        {
            Problem problem;
            int number;

            if (args.Length < 1)
            {
                var e = new Eratosthenes();

                //var count = 0;
                //var start = DateTime.Now;

                foreach (var p in e.OptimizedSieve())
                {
                    //count++;
                    //var delta = (DateTime.Now - start).TotalMilliseconds;
                    //var rate = (double)count / delta;
                    //NonBlockingConsole.WriteLine(p + "\t\t" + rate);

                    NonBlockingConsole.WriteLine(p);
                }

                NonBlockingConsole.Flush();

                return 0;
            }
            else if (!int.TryParse(args[0], out number) || !Problems.TryGetValue(number, out problem))
            {
                Console.WriteLine("Can't find problem " + args[0]);
                Console.WriteLine("Enter a problem number { " + string.Join(", ", Problems.Keys) + " }");
                return 1;
            }

            Console.WriteLine("Solving " + problem.GetType().Name + " " + DateTime.Now);

            var timer = new Stopwatch();
            timer.Start();
            object solution = problem.Solve();
            timer.Stop();

            var flush = new Regex("^-{1,2}f(lush)$",
              RegexOptions.Compiled | RegexOptions.IgnoreCase |
              RegexOptions.Singleline | RegexOptions.CultureInvariant);
            if (args.Any(s => flush.IsMatch(s)))
            {
                NonBlockingConsole.Flush();
            }

            Console.WriteLine("Solved " + problem.GetType().Name + " " + DateTime.Now);
            Console.WriteLine();
            Console.WriteLine("=========================================");
            Console.WriteLine("Solved in " + timer.Elapsed.TotalSeconds + "s");
            Console.WriteLine("=========================================");
            Console.WriteLine(solution);
            Console.WriteLine("=========================================");

            Console.WriteLine();

            return 0;
        }
    }
}
