using System;
using System.Collections.Generic;
using ConstraintOptimization3.Data;
using Google.OrTools.Sat;

namespace ConstraintOptimization3
{
    public class SudokuSolver
    {
        public Sudoku Solve(Sudoku sudoku)
        {
            CpModel model = new CpModel();

            // Initialize field 9x9
            var field = new IntVar[9][];
            for (int x = 0; x < field.Length; x++)
            {
                field[x] = new IntVar[9];
            }

            // Constraint: Every cell contains a number 1-9
            for (int x = 0; x < field.Length; x++)
            {
                for (int y = 0; y < field[x].Length; y++)
                {
                    field[x][y] = model.NewIntVar(1, 9, "grid" + "(" + x + "," + y + ")");
                }
            }

            // Constraints: all cells in each column contain a different value
            for (int x = 0; x < field.Length; x++)
            {
                model.AddAllDifferent(field[x]);
            }

            // Constraint: all cells in each row contain a different value
            for (int y = 0; y < field[0].Length; y++)
            {
                var row = new IntVar[9];
                for (int x = 0; x < field.Length; x++)
                {
                    row[x] = field[x][y];
                }
                model.AddAllDifferent(row);
            }

            // Constraint: all cells in each region contain a different value
            for (int xBegin = 0; xBegin < 7; xBegin += 3)
            {
                for (int yBegin = 0; yBegin < 7; yBegin += 3)
                {
                    List<IntVar> region = new List<IntVar>();
                    for (int xOffset = 0; xOffset < 3; xOffset++)
                    {
                        for (int yOffset = 0; yOffset < 3; yOffset++)
                        {
                            region.Add(field[xBegin + xOffset][yBegin + yOffset]);
                        }
                    }
                    model.AddAllDifferent(region);
                }
            }

            // Load data
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (sudoku.GetCell(x, y) != 0)
                    {
                        model.Add(field[x][y] == sudoku.GetCell(x, y));
                    }
                }
            }

            // Solve
            var solver = new CpSolver();
            var status = solver.Solve(model);

            List<int> solution = new List<int>();
            if (status == CpSolverStatus.Optimal)
            {
                for (int y = 0; y < 9; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        solution.Add((int)solver.Value(field[x][y]));
                    }
                }

                return new Sudoku(solution);
            }

            throw new InvalidOperationException("Sudoku not solvable");
        }
    }
}