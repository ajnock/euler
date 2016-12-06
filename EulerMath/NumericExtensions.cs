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
    }
}
