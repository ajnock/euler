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
    /// The type of integer contained in the sqare
    /// </summary>
    [Flags]
    public enum SquareStatus
    {
        Empty = 0x0,
        Touched = 0x1,
        Prime = 0x11
    }

    public abstract class Spiral
    {
        /// <summary>
        /// Max value of the spiral
        /// </summary>
        public readonly int Max;

        /// <summary>
        /// Square root of <see cref="Max"/>
        /// </summary>
        public readonly int Root;

        protected abstract Color SetPixelColor(int x, int y, SquareStatus status);

        protected abstract SquareStatus SetPixelStatus(int x, int y, int j);

        /// <summary>
        /// Array containing the data the is the spiral. The array is <see cref="Root"/>x<see cref="Root"/>
        /// </summary>
        protected readonly SquareStatus[,] _map;

        public Spiral(int root)
        {
            Root = root;
            Max = Root * Root;
            _map = new SquareStatus[root, root];
        }
        
        /// <summary>
        /// Save the Spiral to disk as a bitmap
        /// </summary>
        /// <param name="file"></param>
        internal void Save(string file)
        {
            using (var bitmap = new Bitmap(Root, Root))
            {
                for (int x = 0; x < Root; x++)
                {
                    for (int y = 0; y < Root; y++)
                    {
                        Color color = SetPixelColor(x, y, _map[x, y]);

                        bitmap.SetPixel(x, y, color);
                    }
                }

                bitmap.Save(file, ImageFormat.Png);
            }
        }

        public virtual void GenerateAndSave(string file)
        {
            using (var bitmap = new Bitmap(Root, Root))
            {
                int n = Root;
                int i = 1;

                Direction dir = Direction.Right;
                int j = i;
                int y = (int)Math.Floor((double)n / 2);
                int x = (n % 2 == 0) ? y - 1 : y; //shift left for even n's

                while (j <= Max)
                {
                    var status = SetPixelStatus(x, y, j);
                    _map[x, y] = status;
                    var color = SetPixelColor(x, y, status);

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

                bitmap.Save(file, ImageFormat.Png);
            }
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

        public virtual void Generate()
        {
            int n = Root;
            int i = 1;

            Direction dir = Direction.Right;
            int j = i;
            int y = (int)Math.Floor((double)n / 2);
            int x = (n % 2 == 0) ? y - 1 : y; //shift left for even n's

            while (j <= Max)
            {
                var status = SetPixelStatus(x, y, j);
                _map[x, y] = status;

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
