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
    public class MongoSpiralMapper : ISpiral
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

        /// <summary>
        /// Array containing the data the is the spiral. The array is <see cref="Root"/>x<see cref="Root"/>
        /// </summary>
        protected readonly SquareStatus[,] _map;

        public MongoSpiralMapper(int root)
        {
            Root = root;
            Max = Root * Root;
            _eratosthenes = new Eratosthenes();
            _primes = _eratosthenes.OptimizedSieve(Max);
            _map = new SquareStatus[root, root];
        }

        /// <summary>
        /// True if the space on the spiral has not been marked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected bool IsEmpty(int x, int y)
        {
            return _map[x, y] == SquareStatus.Empty;
        }

        public void GenerateAndSave(string file)
        {
            int n = Root;
            int i = 1;

            Direction dir = Direction.Right;
            long j = i;
            int y = (int)Math.Floor((double)n / 2);
            int x = (n % 2 == 0) ? y - 1 : y; //shift left for even n's

            while (j <= Max)
            {
                _map[x, y] = SquareStatus.Touched;

                NonBlockingConsole.WriteLine("db.spiralMap.insert({ _id : " + j + ", x : " + x + ", y : " + y + " })");

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

                if (j % 100000 == 0)
                {
                    NonBlockingConsole.Flush();
                }
            }

            NonBlockingConsole.Flush();

            foreach (var p in _primes)
            {
                NonBlockingConsole.WriteLine("db.spiralMap.update({ _id : " + p + " }, { $set : { isPrime : true } })");
            }

            NonBlockingConsole.Flush();
        }
    }
}
