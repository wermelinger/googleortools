using System;
using System.Collections.Generic;
using System.Linq;

using Google.OrTools.Sat;

namespace EmployeeScheduling
{
    class Program
    {
        /// <summary>
        /// In the next example, a hospital supervisor needs to create a schedule for four nurses over a three-day period, subject to the following conditions:
        /// - Each day is divided into three 8-hour shifts.
        /// - Every day, each shift is assigned to a single nurse, and no nurse works more than one shift.
        /// - Each nurse is assigned to at least two shifts during the three-day period.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int numberOfNurses = 4;
            int numberOfShiftsPerDay = 3;
            int numberOfDays = 3;

            var allNurses = Enumerable.Range(0, numberOfNurses);
            var allShiftsPerDay = Enumerable.Range(0, numberOfShiftsPerDay);
            var allDays = Enumerable.Range(0, numberOfDays);

            // Model
            CpModel model = new CpModel();

            // Variables
            IntVar[,,] shift = new IntVar[numberOfNurses, numberOfDays, numberOfShiftsPerDay];
            foreach(int n in allNurses)
            {
                foreach(int d in allDays)
                {
                    foreach(int s in allShiftsPerDay)
                    {
                        shift[n, d, s] = model.NewBoolVar($"shift_n{n}d{d}s{s}");
                    }
                }
            }

            // Constraint: Assign a single nurse to each shift
            foreach (int d in allDays)
            {
                foreach (int s in allShiftsPerDay)
                {
                    IntVar[] tmp = new IntVar[numberOfNurses];
                    foreach (int n in allNurses)
                    {
                        tmp[n] = shift[n, d, s];
                    }
                    model.Add(LinearExpr.Sum(tmp) == 1);
                }
            }

            // Constraint: Each nurse does at most 1 shift per day
            foreach (int n in allNurses)
            {
                foreach (int d in allDays)
                {
                    IntVar[] tmp = new IntVar[numberOfShiftsPerDay];
                    foreach (int s in allShiftsPerDay)
                    {
                        tmp[s] = shift[n, d, s];
                    }
                    model.Add(LinearExpr.Sum(tmp) <= 1);
                }
            }

            // Constraint: Assign evenly
            var minShiftsPerNurse = numberOfDays - 1;
            var maxShiftsPerNurse = numberOfDays;
            foreach (int n in allNurses)
            {
                IntVar[,] tmp = new IntVar[numberOfDays, numberOfShiftsPerDay];
                foreach (int d in allDays)
                {
                    foreach (int s in allShiftsPerDay)
                    {
                        tmp[d, s] = shift[n, d, s];
                    }
                }

                var numberOfShiftsWorked = LinearExpr.Sum(tmp.Cast<IntVar>().ToList());
                model.Add(minShiftsPerNurse <= numberOfShiftsWorked);
                model.Add(numberOfShiftsWorked <= maxShiftsPerNurse);
            }

            // Solve
            CpSolver solver = new CpSolver();

            HashSet<int> to_print = new HashSet<int>();
            to_print.Add(859);
            to_print.Add(2034);
            to_print.Add(5091);
            to_print.Add(7003);
            NurseSolutionObserver cb = new NurseSolutionObserver(shift, numberOfNurses, numberOfDays, numberOfShiftsPerDay, to_print);
            CpSolverStatus status = solver.SearchAllSolutions(model, cb);

            // Statistics.
            Console.WriteLine("Statistics");
            Console.WriteLine(String.Format("  - solve status    : {0}", status));
            Console.WriteLine("  - conflicts       : " + solver.NumConflicts());
            Console.WriteLine("  - branches        : " + solver.NumBranches());
            Console.WriteLine("  - wall time       : " + solver.WallTime() + " ms");
            Console.WriteLine("  - #solutions      : " + cb.SolutionCount());
        }


        public class NurseSolutionObserver : CpSolverSolutionCallback
        {
            public NurseSolutionObserver(IntVar[,,] shifts, int num_nurses, int num_days,
                                         int num_shifts, HashSet<int> to_print)
            {
                shifts_ = shifts;
                num_nurses_ = num_nurses;
                num_days_ = num_days;
                num_shifts_ = num_shifts;
                to_print_ = to_print;
            }

            public override void OnSolutionCallback()
            {
                solution_count_++;
                if (to_print_.Contains(solution_count_))
                {
                    Console.WriteLine(
                        String.Format("Solution #{0}: time = {1:.02} s",
                                      solution_count_, WallTime()));
                    for (int d = 0; d < num_days_; ++d)
                    {
                        Console.WriteLine(String.Format("Day #{0}", d));
                        for (int n = 0; n < num_nurses_; ++n)
                        {
                            for (int s = 0; s < num_shifts_; ++s)
                            {
                                if (BooleanValue(shifts_[n, d, s]))
                                {
                                    Console.WriteLine(
                                        String.Format("  Nurse #{0} is working shift #{1}", n, s));
                                }
                            }
                        }
                    }
                }
            }

            public int SolutionCount()
            {
                return solution_count_;
            }

            private int solution_count_;
            private IntVar[,,] shifts_;
            private int num_nurses_;
            private int num_days_;
            private int num_shifts_;
            private HashSet<int> to_print_;
        }
    }
}
