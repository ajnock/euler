using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EulerMath
{
    public static class ColorExtensions
    {
        public static Color Add(this Color color, Color other)
        {
            //var alpha = Math.Min(color.A - other.A, 255);
            var red = Math.Min(color.R - other.R, 255);
            var green = Math.Min(color.G - other.G, 255);
            var blue = Math.Min(color.B - other.B, 255);

            return Color.FromArgb(255, red, green, blue);
        }
    }
}
