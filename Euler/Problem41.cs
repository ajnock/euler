using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{/// <summary>
 /// Pandigital prime
 /// Problem 41
 /// We shall say that an n-digit number is pandigital if it makes use of all the digits 1 to n exactly once.
 /// For example, 2143 is a 4-digit pandigital and is also prime.
 /// What is the largest n-digit pandigital prime that exists?
 /// </summary>
    class Problem41 : Problem
    {
        public override object Solve()
        {
            var seive = new Eratosthenes();
            var primes = seive.Seive(10000000000);
            long solution = 0;

            foreach (var p in primes)
            {
                if (Euler.IsPandigitial(p))
                {
                    solution = p;
                    Console.WriteLine(p);
                }
            }

            return solution;
        }
    }
}
