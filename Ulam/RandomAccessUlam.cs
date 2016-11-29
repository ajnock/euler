using Euler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ulam
{
    public class RandomAccessUlamSpiral : ISpiral
    {
        /// <summary>
        /// Max value of the spiral
        /// </summary>
        public readonly int Max;

        /// <summary>
        /// Square root of <see cref="Max"/>
        /// </summary>
        public readonly int Root;

        /// <summary>
        /// Prime generator
        /// </summary>
        private readonly Eratosthenes _eratosthenes;
        private readonly IEnumerable<long> _primes;
        private readonly Tuple<int, int>[] _addressBook;

        public RandomAccessUlamSpiral(int root)
        {
            Root = root;
            Max = Root * Root;
            _eratosthenes = new Eratosthenes();
            _primes = _eratosthenes.OptimizedSieve(Max);
            _addressBook = new Tuple<int, int>[Max];
        }

        protected Color SetPixelColor(int x, int y, SquareStatus status)
        {
            switch (status)
            {
                case SquareStatus.Prime:
                    return Color.Black;
                case SquareStatus.Touched:
                    return Color.White;
                default:
                    throw new Exception("Undefined: " + x + ", " + y);
            }
        }

        /// <summary>
        /// True if the space on the spiral has not been marked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected bool IsEmpty(int x, int y, Bitmap bitmap)
        {
            var pixel = bitmap.GetPixel(x, y);
            return pixel.A == 0x00 && pixel.R == 0x00 && pixel.G == 0x00 && pixel.B == 0x00;
        }

        public void GenerateAndSave(string file)
        {
            using (var bitmap = new Bitmap(Root, Root))
            {
                int n = Root;
                int i = 1;

                Direction dir = Direction.Right;
                long j = i;
                int y = (int)Math.Floor((double)n / 2);
                int x = (n % 2 == 0) ? y - 1 : y; //shift left for even n's

                while (j <= Max)
                {
                    _addressBook[j - 1] = new Tuple<int, int>(x, y);

                    var color = SetPixelColor(x, y, SquareStatus.Touched);
                    bitmap.SetPixel(x, y, color);

                    switch (dir)
                    {
                        case Direction.Right:
                            if (x <= n - 1 && IsEmpty(x, y - 1, bitmap) && j > i)
                                dir = Direction.Up;
                            break;
                        case Direction.Up:
                            if (IsEmpty(x - 1, y, bitmap))
                                dir = Direction.Left;
                            break;
                        case Direction.Left:
                            if (x == 0 || IsEmpty(x, y + 1, bitmap))
                                dir = Direction.Down;
                            break;
                        case Direction.Down:
                            if (IsEmpty(x + 1, y, bitmap))
                                dir = Direction.Right;
                            break;
                    }

                    switch (dir)
                    {
                        case Direction.Right:
                            x++;
                            break;
                        case Direction.Up:
                            y--;
                            break;
                        case Direction.Left:
                            x--;
                            break;
                        case Direction.Down:
                            y++;
                            break;
                    }

                    j++;
                }

                foreach (var p in _primes)
                {
                    var location = _addressBook[p - 1];
                    NonBlockingConsole.WriteLine(p + " (" + location.Item1 + ", " + location.Item2 + ")");
                    var color = SetPixelColor(x, y, SquareStatus.Prime);
                    bitmap.SetPixel(location.Item1, location.Item2, color);
                }

                bitmap.Save(file, ImageFormat.Png);
            }
        }
    }
}
