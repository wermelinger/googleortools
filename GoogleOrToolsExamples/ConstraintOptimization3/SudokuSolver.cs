using System;
using System.Collections.Generic;
using ConstraintOptimization3.Data;
using Google.OrTools.Sat;

namespace ConstraintOptimization3
{
    public class SudokuSolver
    {
        public Sudoku Solve(Sudoku s)
        {
            CpModel model = new CpModel();

            // Initialize field (constraint values to 1-9)
            IntVar[][] tab_s = new IntVar[9][];
            for (int i = 0; i < tab_s.Length; i++)
            {
                tab_s[i] = new IntVar[9];
            }

            for (int i = 0; i < tab_s.Length; i++)
            {
                for (int j = 0; j < tab_s[i].Length; j++)
                {
                    tab_s[i][j] = model.NewIntVar(1, 9, "grid" + "(" + i + "," + j + ")");
                }
            }

            // Constraints all differents on rows 
            for (int i = 0; i < tab_s.Length; i++)
            {
                model.AddAllDifferent(tab_s[i]);
            }

            // Constraints all differents on colums 
            IntVar[] tpm = new IntVar[9];
            for (int j = 0; j < tab_s[0].Length; j++)
            {
                for (int i = 0; i < tab_s.Length; i++)
                {
                    tpm[i] = tab_s[i][j];
                }
                model.AddAllDifferent(tpm);
                Array.Clear(tpm, 0, tpm.Length);
            }

            // Constraint all differents on cells 
            List<IntVar> ls = new List<IntVar>();
            for (int i = 0; i < 7; i += 3)
            {
                for (int j = 0; j < 7; j += 3)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            ls.Add(tab_s[i + k][j + l]);
                        }

                    }
                    model.AddAllDifferent(ls);
                    ls.Clear();
                }
            }

            // Initial Value
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s.GetCell(i, j) != 0)
                    {
                        model.Add(tab_s[i][j] == s.GetCell(i, j));
                    }
                }
            }

            // Solve
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);
            
            List<int> solution = new List<int>();
            if (status == CpSolverStatus.Feasible || status == CpSolverStatus.Optimal)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {

                        solution.Add((int)solver.Value(tab_s[i][j]));
                    }
                }
            }

            Sudoku resolution = new Sudoku(solution);
            return resolution;
        }
    }
}