using System;

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

            var k = long.Parse(args[1]);

            ISpiral ulam;

            switch (args[0].ToLower())
            {
                case "image":
                    int max = (int)Math.Min(int.MaxValue, k);
                    ulam = new BitmapSpiral(max);
                    break;
                case "mongo":
                    ulam = new MongoSpiral(k);
                    break;
                default:
                    throw new ArgumentException("Ulam.exe (mongo|image) max [./ulam100x100.png]");
            }

            try
            {
                if (args.Length >= 3)
                {
                    ulam.GenerateAndSave(args[2]).Wait();
                }
                else
                {
                    ulam.GenerateAndSave().Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
