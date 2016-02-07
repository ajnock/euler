using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    /// <summary>
    /// Euler specific functions
    /// </summary>
    public static class Euler
    {
        public static bool IsPandigitial(long p)
        {
            var digits = p.ToString();
            int n = digits.Length;
            for (int i = 1; i <= n; i++)
            {
                if (!digits.Contains(i.ToString()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
