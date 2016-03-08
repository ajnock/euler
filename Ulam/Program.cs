using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ulam
{
    class Program
    {
        static void Main(string[] args)
        {
            int k = 100;
            if (args.Length == 1)
            {
                k = int.Parse(args[0]);
            }

            var ulam = new Ulam(k);
            ulam.Generate();
            ulam.Save(@"D:\Ulam" + k + "x" + k + ".bmp", ImageFormat.Bmp);
        }
    }
}
