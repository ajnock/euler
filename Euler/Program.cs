using System;
using System.Diagnostics;

namespace Euler
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem problem = new Problem111();

            var timer = new Stopwatch();
            timer.Start();
            object solution = problem.Solve();
            timer.Stop();

            Console.WriteLine();
            Console.WriteLine("========================================= " + timer.Elapsed.TotalSeconds + "s");
            Console.WriteLine(solution);
            Console.ReadLine();
        }
    }
}
