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
    class Ulam
    {
        private readonly Eratosthenes _eratosthenes;
        private readonly int _max;
        private readonly int _root;
        private readonly SquareStatus[,] _key;

        internal void Save(string file, ImageFormat format)
        {
            using (var bitmap = new Bitmap(_root, _root))
            {
                for (int x = 0; x < _root; x++)
                {
                    for (int y = 0; y < _root; y++)
                    {
                        Color color;
                        switch (_key[x, y])
                        {
                            case SquareStatus.Prime:
                                color = Color.Black;
                                break;
                            case SquareStatus.Touched:
                                color = Color.White;
                                break;
                            default:
                                throw new Exception("Undefined: " + x + ", " + y);
                        }

                        bitmap.SetPixel(x, y, color);
                    }
                }

                bitmap.Save(file, format);
            }
        }

        public Ulam(int root)
        {
            _eratosthenes = new Eratosthenes();
            _root = root;
            _max = _root * _root;
            _key = new SquareStatus[root, root];
        }

        private enum Direction
        {
            Right, Up, Left, Down
        }

        [Flags]
        private enum SquareStatus
        {
            Empty = 0x0,
            Touched = 0x1,
            Prime = 0x11
        }

        private bool IsEmpty(int x, int y)
        {
            return _key[x, y] == SquareStatus.Empty;
        }

        public void Generate()
        {
            var primes = _eratosthenes.OptimizedSieveSorted(_max).GetEnumerator();
            primes.MoveNext();

            int n = _root;
            int i = 1;

            Direction dir = Direction.Right;
            int j = i;
            int y = (int)Math.Floor((double)n / 2);
            int x = (n % 2 == 0) ? y - 1 : y; //shift left for even n's

            while (j <= _max)
            {
                var status = SquareStatus.Touched;
                if (j == primes.Current)
                {
                    NonBlockingConsole.WriteLine(j);
                    status |= SquareStatus.Prime;
                    primes.MoveNext();
                }

                _key[x, y] = status;

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
    }
}
