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
        private readonly Bitmap _map;
        private readonly int _root;

        internal void Save(string file, ImageFormat format)
        {
            _map.Save(file, format);
        }

        public Ulam(int root)
        {
            _eratosthenes = new Eratosthenes();
            _root = root;
            _max = _root * _root;
            _map = new Bitmap(_root, _root);
        }

        enum Direction
        {
            Right, Up, Left, Down
        }

        private static readonly Color Prime = Color.Black;
        private static readonly Color Composite = Color.White;
        private static readonly Color Empty = Color.Transparent;

        private bool IsEmpty(int x, int y)
        {
            var c = _map.GetPixel(x, y);
            return c.R == Empty.R && c.G == Empty.G && c.B == Empty.B && c.A == Empty.A;
        }

        public async void Generate()
        {
            var primes = _eratosthenes.OptimizedSieve(_max).OrderBy(p => p).ToList().GetEnumerator();
            primes.MoveNext();

            for (int k = 0; k < _root; k++)
            {
                for (int l = 0; l < _root; l++)
                {
                    _map.SetPixel(l, k, Empty);
                }
            }

            int n = _root;
            int i = 1;

            Direction dir = Direction.Right;
            int j = i;
            int y = (int)Math.Floor((double)n / 2);
            int x = y; // assume odd size

            while (j <= _max)
            {
                var color = Composite;
                if (j == primes.Current)
                {
                    NonBlockingConsole.WriteLine(j);
                    color = Prime;
                    primes.MoveNext();
                }

                //var c = _map.GetPixel(x, y);
                _map.SetPixel(x, y, color);

                switch (dir)
                {
                    case Direction.Right:
                        if (x <= (n - 1) && IsEmpty(x, y - 1) && j > i)
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
