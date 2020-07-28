using System;
using Google.OrTools.ConstraintSolver;

namespace ConstraintOptimization
{
    class CpSimple
    {
        static void Main(string[] args)
        {
            // Solver
            Solver solver = new Solver("CpSimple");

            // Variables
            IntVar x = solver.MakeIntVar(0, 2, "x");
            IntVar y = solver.MakeIntVar(0, 2, "y");
            IntVar z = solver.MakeIntVar(0, 2, "z");

            // Constraint 0: x != y
            solver.Add(solver.MakeAllDifferent(new IntVar[] { x, y }));

            solver.Add(solver.MakeLess(y, z));

            // Solve
            DecisionBuilder db = solver.MakePhase(new IntVar[] { x, y, z }, Solver.CHOOSE_FIRST_UNBOUND, Solver.ASSIGN_MIN_VALUE);

            // Print
            int count = 0;
            solver.NewSearch(db);
            while(solver.NextSolution())
            {
                count++;
                Console.WriteLine($"Solution: {count}\n x={x.Value()} y={y.Value()} z={z.Value()}");
            }

            solver.EndSearch();

        }
    }
}
