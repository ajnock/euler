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
        /// <summary>
        /// Prime generator
        /// </summary>
        private readonly Eratosthenes _eratosthenes;

        /// <summary>
        /// Max value of the spiral
        /// </summary>
        private readonly int _max;

        /// <summary>
        /// Square root of <see cref="_max"/>
        /// </summary>
        private readonly int _root;

        /// <summary>
        /// Array containing the data the is the spiral. The array is <see cref="_root"/>x<see cref="_root"/>
        /// </summary>
        private readonly SquareStatus[,] _map;

        /// <summary>
        /// Save the Spiral to disk as a bitmap
        /// </summary>
        /// <param name="file"></param>
        /// <param name="format"></param>
        internal void Save(string file)
        {
            using (var bitmap = new Bitmap(_root, _root, PixelFormat.Format16bppRgb555))
            {
                for (int x = 0; x < _root; x++)
                {
                    for (int y = 0; y < _root; y++)
                    {
                        Color color;
                        switch (_map[x, y])
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

                bitmap.Save(file, ImageFormat.Bmp);
            }
        }

        /// <summary>
        /// Create a new <see cref="Ulam"/>. Each side will be of <paramref name="root"/> length.
        /// </summary>
        /// <param name="root">Square root of the max value</param>
        public Ulam(int root)
        {
            _eratosthenes = new Eratosthenes();
            _root = root;
            _max = _root * _root;
            _map = new SquareStatus[root, root];
        }

        /// <summary>
        /// Direction the spiral is moving in Cartesian coordinates
        /// </summary>
        private enum Direction
        {
            Right, Up, Left, Down
        }

        /// <summary>
        /// The type of integer contained in the sqare
        /// </summary>
        [Flags]
        private enum SquareStatus
        {
            Empty = 0x0,
            Touched = 0x1,
            Prime = 0x11
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

        private delegate void FrameAction(SquareStatus[,] map);

        public void GenerateGif(string file)
        {

        }

        /// <summary>
        /// Generates the spiral in <see cref="_map"/>
        /// </summary>
        public void Generate()
        {
            Generate(null);
        }

        private void Generate(FrameAction actOnFrame = null)
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

                _map[x, y] = status;

                if (actOnFrame != null)
                {
                    actOnFrame(_map);
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
    }
}
