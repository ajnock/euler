using System;
using System.Collections.Generic;
using System.Text;

namespace Euler
{
    public struct Sudoku : IEqualityComparer<Sudoku>, IEquatable<Sudoku>, ICloneable
    {
        public int EulerValue
        {
            get
            {
                return _board[0, 0] + _board[1, 0] + _board[2, 0];
            }
        }

        private static readonly HashSet<Sudoku> cache = new HashSet<Sudoku>();

        public bool IsSolved
        {
            get
            {
                if (!cache.Add(this))
                {
                    return false;
                }

                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        if (_board[x, y] == 0)
                        {
                            return false;
                        }
                    }
                }

                cache.Remove(this);

                return true;
            }
        }

        public bool Set(int x, int y, int value)
        {
            if (_board[x, y] == 0 && Test(x, y, value))
            {
                _board[x, y] = value;
                return true;
            }

            return false;
        }

        public IEnumerable<Sudoku> Neighbors
        {
            get
            {
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        if (_board[x, y] == 0)
                        {
                            for (int i = 1; i <= 9; i++)
                            {
                                var s = new Sudoku(this);
                                if (s.Set(x, y, i))
                                {
                                    yield return s;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool Test(int x, int y, int value)
        {
            var set = new HashSet<int>();

            for (int i = 0; i < 9; i++)
            {
                if (i == y)
                    set.Add(value);
                else if (_board[x, i] > 0 && !set.Add(_board[x, i]))
                {
                    return false;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                if (i == x)
                    set.Add(value);
                else if (_board[i, y] > 0 && !set.Add(_board[i, y]))
                {
                    return false;
                }
            }

            return true;
        }
        public int Sum
        {
            get
            {
                int sum = 0;
                foreach (int i in _board)
                {
                    if (i > 0)
                    {
                        sum++;
                    }
                }

                return sum;
            }
        }
        private readonly int[,] _board;

        public Sudoku(IList<string> lines)
        {
            if (lines.Count != 9)
            {
                throw new ArgumentException();
            }
            _board = new int[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    _board[x, y] = int.Parse(lines[y][x].ToString());
                }
            }
        }

        public Sudoku(Sudoku sudoku)
        {
            _board = (int[,])sudoku._board.Clone();
        }

        public bool Equals(Sudoku x, Sudoku y)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (x._board[i, j] != y._board[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Sudoku)
            {
                return Equals((Sudoku)obj);
            }
            return false;
        }

        public int GetHashCode(Sudoku obj)
        {
            return obj.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    builder.Append(_board[x, y]);
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }

        public bool Equals(Sudoku other)
        {
            return Equals(this, other);
        }

        public object Clone()
        {
            return new Sudoku(this);
        }

        internal bool IsSet(int x, int y)
        {
            return _board[x, y] != 0;
        }
    }

}
