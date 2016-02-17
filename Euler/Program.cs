using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            { 58, new Problem58() }
        };
        static int Main(string[] args)
        {
            Problem problem;
            int number;

            if (args.Length < 1)
            {
                //Console.WriteLine("Enter a problem number { " + string.Join(", ", Problems.Keys) + " }");
                //return -1;

                var e = new Eratosthenes();
                foreach (var p in e.Sieve())
                {
                    Console.WriteLine(p);
                }

                return -1;
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
