﻿using ConstraintOptimization3.Data;
using System;

namespace ConstraintOptimization3
{
    class SudokuProgram
    {
        static void Main(string[] args)
        {
            int[] initial_grid = new int[] { 0, 6, 0,   0, 5, 0,   0, 2, 0,
                                             0, 0, 0,   3, 0, 0,   0, 9, 0,
                                             7, 0, 0,   6, 0, 0,   0, 1, 0,

                                             0, 0, 6,   0, 3, 0,   4, 0, 0,
                                             0, 0, 4,   0, 7, 0,   1, 0, 0,
                                             0, 0, 5,   0, 9, 0,   8, 0, 0,

                                             0, 4, 0,   0, 0, 1,   0, 0, 6,
                                             0, 3, 0,   0, 0, 8,   0, 0, 0,
                                             0, 2, 0,   0, 4, 0,   0, 5, 0 };

            Sudoku sudoku = new Sudoku(initial_grid);
            Console.WriteLine(sudoku.ToString());

            SudokuSolver solver = new SudokuSolver();
            Sudoku solution = solver.Solve(sudoku);

            Console.WriteLine(solution.ToString());
        }
    }
}
