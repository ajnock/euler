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
    /// <summary>
    /// Direction the spiral is moving in Cartesian coordinates
    /// </summary>
    public enum Direction
    {
        Invalid = -1,
        Right, Up, Left, Down
    }

    public class BitmapSpiral : Spiral
    {
        /// <summary>
        /// Prime generator
        /// </summary>
        private readonly Eratosthenes _eratosthenes;
        private readonly IEnumerator<long> _primes;

        /// <summary>
        /// Create a new <see cref="BitmapSpiral"/>. Each side will be of <paramref name="root"/> length.
        /// </summary>
        /// <param name="root">Square root of the max value</param>
        public BitmapSpiral(int root)
            : base(root)
        {
            _eratosthenes = new Eratosthenes();
            _primes = _eratosthenes.OptimizedSieveSorted(Max).GetEnumerator();
        }

        public override void Generate()
        {
            _primes.MoveNext();

            base.Generate();
        }
        public override async Task GenerateAndSave(string file)
        {
            _primes.MoveNext();

            await base.GenerateAndSave(file);
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
            var status = SquareStatus.Touched;
            if (j == _primes.Current)
            {
                NonBlockingConsole.WriteLine(j);
                status |= SquareStatus.Prime;
                _primes.MoveNext();
            }

            return status;
        }

        public bool[,] GetMap()
        {
            var map = new bool[Root, Root];
            for (int x = 0; x < Root; x++)
            {
                for (int y = 0; y < Root; y++)
                {
                    map[x, y] = _map[x, y] == SquareStatus.Prime ? true : false;
                }
            }

            return map;
        }
    }
}
