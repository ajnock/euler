using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    /// <summary>
    /// Su Doku (Japanese meaning number place) is the name given to a popular puzzle concept. Its origin is unclear, but credit must be attributed to Leonhard Euler who invented a similar, and much more difficult, puzzle idea called Latin Squares. The objective of Su Doku puzzles, however, is to replace the blanks (or zeros) in a 9 by 9 grid in such that each row, column, and 3 by 3 box contains each of the digits 1 to 9. Below is an example of a typical starting puzzle grid and its solution grid.
    /// A well constructed Su Doku puzzle has a unique solution and can be solved by logic, although it may be necessary to employ "guess and test" methods in order to eliminate options(there is much contested opinion over this). The complexity of the search determines the difficulty of the puzzle; the example above is considered easy because it can be solved by straight forward direct deduction.
    /// The 6K text file, sudoku.txt (right click and 'Save Link/Target As...'), contains fifty different Su Doku puzzles ranging in difficulty, but all with unique solutions(the first puzzle in the file is the example above).
    /// By solving all fifty puzzles find the sum of the 3-digit numbers found in the top left corner of each solution grid; for example, 483 is the 3-digit number found in the top left corner of the solution grid above.
    /// </summary>
    public class Problem96 : Problem
    {
        private IEnumerable<Sudoku> GetBoards()
        {
            var games = new List<Sudoku>();
            IList<string> board = new List<string>();
            int i = 0;
            foreach (var line in File.ReadLines("p096_sudoku.txt"))
            {
                if (i % 10 != 0)
                {
                    board.Add(line);
                }

                i++;

                if (i % 10 == 0)
                {
                    yield return new Sudoku(board);
                    board = new List<string>();
                }
            }
        }

        int depth = 0;
        private bool Solve(Sudoku sudoku, out Sudoku solution)
        {
            if (sudoku.IsSolved)
            {
                solution = sudoku;

                Console.WriteLine();
                Console.WriteLine(solution);
                Console.WriteLine();

                return true;
            }
            
            foreach (var next in sudoku.Neighbors)
            {
                depth++;
                if (Solve(next, out solution))
                {
                    Console.WriteLine();
                    Console.WriteLine(solution);
                    Console.WriteLine();

                    return true;
                }
                depth--;
            }

            if (depth > maxDepth)
            {
                maxDepth = depth;
                Console.WriteLine(DateTime.Now.ToString() + "\t" + depth);
            }

            solution = new Sudoku();
            return false;
        }
        int maxDepth = 0;
        public override object Solve()
        {
            List<Sudoku> solutions = new List<Sudoku>();
            IEnumerable<Sudoku> boards = GetBoards();
            foreach (var board in boards)
            {
                Sudoku s;
                if (Solve(board, out s))
                {
                    solutions.Add(s);
                }
                else
                {
                    throw new Exception("Unsolvable");
                }
            }

            return solutions.Sum(s => s.EulerValue);
        }

        private Sudoku Solve(Sudoku sudoku)
        {
            depth = 0;

            for (int i = 1; i <= 9; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    for (int k = 1; k <= 9; k++)
                    {
                        var s = new Sudoku(sudoku);
                        if (s.TrySet(0, 0, i) && s.TrySet(1, 0, j) && s.TrySet(2, 0, k))
                        {
                            Console.WriteLine(s.EulerValue);

                            Sudoku solution;
                            if (Solve(s, out solution))
                            {
                                return solution;
                            }
                        }
                    }
                }
            }


            return null;
        }
    }
}
