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
            // Initialize fields 9x9
            var fields = new IntVar[9][];
            for (int x = 0; x < fields.Length; x++)
            {
                fields[x] = new IntVar[9];
            }

            var model = new CpModel();

            // Constraint: Every cell contains a number 1-9
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    fields[x][y] = model.NewIntVar(1, 9, $"{x}-{y}");
                }
            }

            // Constraints: all cells in each column contain a different value
            foreach (var column in FieldsHelper.GetColumns(fields))
            {
                model.AddAllDifferent(column);
            }

            // Constraint: all cells in each row contain a different value
            foreach (var row in FieldsHelper.GetRows(fields))
            {
                model.AddAllDifferent(row);
            }

            // Constraint: all cells in each box contain a different value
            foreach (var box in FieldsHelper.GetBoxes(fields))
            {
                model.AddAllDifferent(box);
            }

            // Add constraints of our specific sudoku
            foreach (var cell in sudoku.GetCellsWithValue())
            {
                model.Add(fields[cell.X][cell.Y] == cell.Value);
            }

            // Solve
            var solver = new CpSolver();
            var status = solver.Solve(model);
            if (status == CpSolverStatus.Optimal)
            {
                var solution = FieldsHelper.GetFieldValuesFromSolver(solver, fields);
                return new Sudoku(solution);
            }

            throw new InvalidOperationException("Sudoku not solvable");
        }
    }
}