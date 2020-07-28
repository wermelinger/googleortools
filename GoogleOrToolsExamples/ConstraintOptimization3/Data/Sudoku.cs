﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConstraintOptimization3.Data
{
    public class Sudoku
    {
        /// <summary>
        /// The empty constructor assumes no mask
        /// </summary>
        public Sudoku()
        {
        }

        /// <summary>
        /// constructor that initializes the board with 81 cells
        /// </summary>
        /// <param name="cells"></param>
        public Sudoku(IEnumerable<int> cells)
        {
            var enumerable = cells.ToList();
            if (enumerable.Count != 81)
            {
                throw new ArgumentException("Sudoku should have exactly 81 cells", nameof(cells));
            }
            Cells = new List<int>(enumerable);
        }

        // We use a list for easier access to cells,

        /// <summary>
        /// Easy access by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <returns>value of the cell</returns>
        public int GetCell(int x, int y)
        {
            return Cells[9 * x + y];
        }

        /// <summary>
        /// Easy setter by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <param name="value">value of the cell to set</param>
        public void SetCell(int x, int y, int value)
        {
            Cells[9 * x + y] = value;
        }

        /// <summary>
        /// Sudoku cells are initialized with zeros standing for empty cells
        /// </summary>
        public IList<int> Cells { get; set; } = Enumerable.Repeat(0, 81).ToList();

        /// <summary>
        /// Displays a Sudoku in an easy to read format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var lineSep = new string('-', 31);

            var output = new StringBuilder();
            output.Append(lineSep);
            output.AppendLine();

            for (int row = 1; row <= 9; row++)
            {
                // we start each line with |
                output.Append("| ");
                for (int column = 1; column <= 9; column++)
                {
                    // we obtain the 81-cell index from the 9x9 row/column index
                    var value = Cells[(row - 1) * 9 + (column - 1)];
                    output.Append(value);
                    //we identify boxes with | within lines
                    output.Append(column % 3 == 0 ? " | " : "  ");
                }

                output.AppendLine();
                //we identify boxes with - within columns
                if (row % 3 == 0)
                {
                    output.Append(lineSep);
                }

                output.AppendLine();
            }

            return output.ToString();
        }
        /*
        /// <summary>
        /// Evaluates a single Sudoku board by counting the duplicates in rows, boxes
        /// and the digits differing from the target mask.
        /// </summary>
        /// <param name="testSudoku">the board to evaluate</param>
        /// <returns>the number of mistakes the Sudoku contains.</returns>
        public double Evaluate(Sudoku testSudoku)
        {
            // We use a large lambda expression to count duplicates in rows, columns and boxes
            var cells = testSudoku.Cells.Select((c, i) => new { index = i, cell = c });
            var toTest = cells.GroupBy(x => x.index / 9).Select(g => g.Select(c => c.cell)) // rows
              .Concat(cells.GroupBy(x => x.index % 9).Select(g => g.Select(c => c.cell))) //columns
              .Concat(cells.GroupBy(x => x.index / 27 * 27 + x.index % 9 / 3 * 3).Select(g => g.Select(c => c.cell))); //boxes
            var toReturn = -toTest.Sum(test => test.GroupBy(x => x).Select(g => g.Count() - 1).Sum()); // Summing over duplicates
            toReturn -= cells.Count(x => _targetSudoku.Cells[x.index] > 0 && _targetSudoku.Cells[x.index] != x.cell); // Mask
            return toReturn;
        }
        */
        /// <summary>
        /// Parses a single Sudoku
        /// </summary>
        /// <param name="sudokuAsString">the string representing the sudoku</param>
        /// <returns>the parsed sudoku</returns>
        public Sudoku Parse(string sudokuAsString)
        {
            return ParseMulti(new[] { sudokuAsString })[0];
        }

        /// <summary>
        /// Parses a file with one or several sudokus
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<Sudoku> ParseFile(string fileName)
        {
            return ParseMulti(File.ReadAllLines(fileName));
        }

        /// <summary>
        /// Parses a list of lines into a list of sudoku, accounting for most cases usually encountered
        /// </summary>
        /// <param name="lines">the lines of string to parse</param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<Sudoku> ParseMulti(string[] lines)
        {
            var toReturn = new List<Sudoku>();
            var cells = new List<int>(81);
            // we ignore lines not starting with a sudoku character
            foreach (var line in lines.Where(l => l.Length > 0
                                                 && IsSudokuChar(l[0])))
            {
                foreach (char c in line)
                {
                    //we ignore lines not starting with cell chars
                    if (IsSudokuChar(c))
                    {
                        if (char.IsDigit(c))
                        {
                            // if char is a digit, we add it to a cell
                            cells.Add((int)char.GetNumericValue(c));
                        }
                        else
                        {
                            // if char represents an empty cell, we add 0
                            cells.Add(0);
                        }
                    }
                    // when 81 cells are entered, we create a sudoku and start collecting cells again.
                    if (cells.Count == 81)
                    {
                        toReturn.Add(new Sudoku() { Cells = new List<int>(cells) });
                        // we empty the current cell collector to start building a new Sudoku
                        cells.Clear();
                    }

                }
            }

            return toReturn;
        }


        /// <summary>
        /// identifies characters to be parsed into sudoku cells
        /// </summary>
        /// <param name="c">a character to test</param>
        /// <returns>true if the character is a cell's char</returns>
        private static bool IsSudokuChar(char c)
        {
            return char.IsDigit(c) || c == '.' || c == 'X' || c == '-';
        }
    }
}