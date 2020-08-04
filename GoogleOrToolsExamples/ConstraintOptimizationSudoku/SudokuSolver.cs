using System;
using System.Collections.Generic;
using ConstraintOptimizationSudoku.Data;
using Google.OrTools.Sat;

namespace ConstraintOptimizationSudoku
{
    public class SudokuSolver
    {
        public Sudoku Solve(Sudoku sudoku)
        {
            // Initialize field 9x9
            var field = new IntVar[9][];
            for (int x = 0; x < field.Length; x++)
            {
                field[x] = new IntVar[9];
            }

            var model = new CpModel();

            // Constraint: Every cell contains a number 1-9
            for (int x = 0; x < field.Length; x++)
            {
                for (int y = 0; y < field[x].Length; y++)
                {
                    // TODO: Add constraint for each field
                    field[x][y] = model.NewIntVar(1, 9, $"{x}-{y}");
                }
            }

            // Constraints: all cells in each column contain a different value
            for (int x = 0; x < field.Length; x++)
            {
                // hint: 2 dimensional array -> list of y-positions must be different 
                var yPositions = field[x];
                model.AddAllDifferent(yPositions);
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
                    var region = new List<IntVar>();
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

            // Add constraints of our specific sudoku
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

            var solution = new List<int>();
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