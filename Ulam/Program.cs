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
            if (args.Length != 2)
            {
                throw new ArgumentException("Ulam.exe 100 ./ulam100x100.png");
            }

            int k = int.Parse(args[0]);

            ISpiral ulam = new MongoSpiralMapper(k);

            //ulam.Generate();
            //ulam.Save(args[1]);

            ulam.GenerateAndSave(args[1]);
        }
    }
}
