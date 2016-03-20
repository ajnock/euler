﻿using System;
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
            int k = int.Parse(args[0]);

            var ulam = new UlamSpiral(k);

            ulam.Generate();
            ulam.Save(args[1]);
        }
    }
}
