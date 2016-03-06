using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ulam
{
    class Program
    {
        static void Main(string[] args)
        {
            var ulam = new Ulam(100);
            ulam.Generate();
            ulam.Save(@"D:\Ulam.bmp");
        }
    }
}
