﻿using System;
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
            int max = 7654321;
            var primes = seive.Seive(max / 3).ToList(); ;

            for (int i = max; i > 0; i -= 2)
            {
                if (Euler.IsPandigitial(i) && !primes.Any(p => i % p == 0))
                {
                    return i;
                }
            }

            return null;
        }
    }
}