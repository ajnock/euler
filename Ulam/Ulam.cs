using EulerMath;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        internal void Save(string file)
        {
            _map.Save(file);
        }

        public Ulam(int k)
        {
            _eratosthenes = new Eratosthenes();
            _root = 2 * k + 1;
            _max = _root * _root;
            _map = new Bitmap(_root, _root);
        }

        enum Direction
        {
            Right, Up, Left, Down
        }

        private static readonly Color Prime = Color.Green;
        private static readonly Color Composite = Color.White;
        private static readonly Color Empty = Color.Black;

        public void Generate()
        {
            var primes = _eratosthenes.OptimizedSieve(_max).ToList();

            int n = _max;
            int i = 1;

            Direction dir = Direction.Right;
            int j = i;
            int y = n / 2;
            int x = (n % 2 == 0) ? y - 1 : y; //shift left for even n's
            while (j <= ((n * n) - 1 + i))
            {
                var color = primes.Contains(j) ? Prime : Composite;
                _map.SetPixel(x, y, color);

                switch (dir)
                {
                    case Direction.Right:
                        if (x <= (n - 1) && _map.GetPixel(y - 1, x) == Empty && j > i)
                            dir = Direction.Up;
                        break;
                    case Direction.Up:
                        if (_map.GetPixel(y, x - 1) == Empty)
                            dir = Direction.Left;
                        break;
                    case Direction.Left:
                        if (x == 0 || _map.GetPixel(y + 1, x) == Empty)
                            dir = Direction.Down;
                        break;
                    case Direction.Down:
                        if (_map.GetPixel(y, x + 1) == Empty)
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
