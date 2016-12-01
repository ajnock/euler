using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EulerMath
{
    public static class NumericExtensions
    {
        public static long ToLong(this ulong u)
        {
            ulong l = u - (ulong)long.MaxValue;
            return (long)l;
        }

        public static ulong ToULong(this long l)
        {
            ulong u = (ulong)l + (ulong)long.MaxValue;
            return (ulong)u;
        }
    }
}
