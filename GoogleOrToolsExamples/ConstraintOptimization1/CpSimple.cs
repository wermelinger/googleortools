using ConstraintOptimization1;
using Google.OrTools.Sat;

namespace ConstraintOptimization
{
    class CpSimple
    {
        static void Main(string[] args)
        {
            // Define model
            CpModel model = new CpModel();

            // Variables x,y,z with constraints: 0 - 2
            IntVar x = model.NewIntVar(0, 2, "x");
            IntVar y = model.NewIntVar(0, 2, "y");
            IntVar z = model.NewIntVar(0, 2, "z");

            // Constraint: x != y
            model.AddAllDifferent(new IntVar[] { x, y });

            // Constraint: y < z
            model.Add(y < z);

            // Solve: all feasible solutions
            CpSolver solver = new CpSolver();
            solver.SearchAllSolutions(model, new VarArraySolutionPrinter(new IntVar[] { x, y, z }));
        }
    }
}
