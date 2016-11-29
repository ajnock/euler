using Euler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ulam
{
    public class RandomAccessUlamSpiral : Spiral
    {
        /// <summary>
        /// Prime generator
        /// </summary>
        private readonly Eratosthenes _eratosthenes;
        private readonly IEnumerable<long> _primes;
        private readonly long[] _squares;
        public RandomAccessUlamSpiral(int root)
            : base(root)
        {
            _eratosthenes = new Eratosthenes();
            _primes = _eratosthenes.OptimizedSieve(Max);
        }

        protected override Color SetPixelColor(int x, int y, SquareStatus status)
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

        protected override SquareStatus SetPixelStatus(int x, int y, int j)
        {
            return SquareStatus.Empty;
        }

        /// <summary>
        /// True if the space on the spiral has not been marked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsEmpty(int x, int y)
        {
            return _map[x, y] == SquareStatus.Empty;
        }

        public void SetMap(long p)
        {
            int n = Root;
            int i = 1;

            Direction dir = Direction.Right;
            int j = i;
            int y = (int)Math.Floor((double)n / 2);
            int x = (n % 2 == 0) ? y - 1 : y; //shift left for even n's

            while (j <= Max)
            {
                if (j == p)
                {
                    NonBlockingConsole.WriteLine(p);
                    _map[x, y] = SquareStatus.Prime;
                    return;
                }

                switch (dir)
                {
                    case Direction.Right:
                        if (x <= n - 1 && IsEmpty(x, y - 1) && j > i)
                            dir = Direction.Up;
                        break;
                    case Direction.Up:
                        if (IsEmpty(x - 1, y))
                            dir = Direction.Left;
                        break;
                    case Direction.Left:
                        if (x == 0 || IsEmpty(x, y + 1))
                            dir = Direction.Down;
                        break;
                    case Direction.Down:
                        if (IsEmpty(x + 1, y))
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
        }

        public override void GenerateAndSave(string file)
        {
            Parallel.ForEach(_primes, p => { SetMap(p); });

            using (var bitmap = new Bitmap(Root, Root))
            {
                for (int x = 0; x < Root; x++)
                {
                    for (int y = 0; y < Root; y++)
                    {
                        if (_map[x, y] != SquareStatus.Prime)
                        {
                            _map[x, y] |= SquareStatus.Touched;
                        }

                        var color = SetPixelColor(x, y, _map[x, y]);
                        bitmap.SetPixel(x, y, color);
                    }
                }

                bitmap.Save(file, ImageFormat.Png);
            }
        }
    }
}
