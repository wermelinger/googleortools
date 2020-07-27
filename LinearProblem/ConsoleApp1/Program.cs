using System;
using Google.OrTools.LinearSolver;

namespace ConsoleApp1
{
    class Program
    {
        /// <summary>
        /// Maximize 3x + y subject to the following constraints
        /// 0	≤	x	≤	1
        /// 0	≤	y	≤	2
        /// x + y	≤	2
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Solver
            Solver solver = Solver.CreateSolver("SimpleLpProgram", "GLOP_LINEAR_PROGRAMMING");

            // Variables
            Variable x = solver.MakeIntVar(0.0, 1.0, "x");
            Variable y = solver.MakeIntVar(0.0, 2.0, "y");

            // Constraint
            Constraint xyLowerTwo = solver.MakeConstraint(0.0, 2.0, "xyLowerTwo");
            xyLowerTwo.SetCoefficient(x, 1);
            xyLowerTwo.SetCoefficient(y, 1);

            // Objective
            Objective objective = solver.Objective();
            objective.SetCoefficient(x, 3);
            objective.SetCoefficient(y, 1);
            objective.SetMaximization();

            // Solve
            solver.Solve();
            Console.WriteLine($"Objective value: {solver.Objective().Value()}");
            Console.WriteLine($"x = {x.SolutionValue()}");
            Console.WriteLine($"y = {y.SolutionValue()}");
        }
    }
}
