using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem problem = new Problem41();

            object solution = problem.Solve();

            Console.WriteLine();
            Console.WriteLine("=========================================");
            Console.WriteLine(solution);
            Console.ReadLine();
        }
    }
}
