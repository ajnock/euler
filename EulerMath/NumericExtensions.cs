using System;
using System.Collections.Generic;
using System.Linq;

namespace EulerMath
{
    public static class NumericExtensions
    {
        public static long ToLong(this ulong u)
        {
            ulong l = u - long.MaxValue;
            return (long)l;
        }

        public static ulong ToULong(this long l)
        {
            ulong u = (ulong)l + long.MaxValue;
            return u;
        }

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
            this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }

        public static string ToHumanReadableString(this TimeSpan t)
        {
            if (t.TotalSeconds <= 10)
            {
                return t.TotalMilliseconds.ToString("F") + " ms";
            }
            if (t.TotalMinutes <= 1)
            {
                return t.TotalSeconds.ToString("F") + " seconds";
            }
            if (t.TotalHours <= 1)
            {
                return t.TotalMinutes.ToString("F") + " minutes";
            }
            if (t.TotalDays <= 1)
            {
                return t.TotalHours.ToString("F") + " hours";
            }

            return t.TotalDays.ToString("F") + " days";
        }
    }
}
