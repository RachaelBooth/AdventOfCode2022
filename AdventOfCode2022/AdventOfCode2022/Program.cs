using AoCHelper;
using System.Reflection;

namespace AdventOfCode2022
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = Assembly.GetEntryAssembly().GetTypes().Where(type => 
                typeof(BaseProblem).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            Solver.SolveLast(options => { options.ShowConstructorElapsedTime = true; options.ShowTotalElapsedTimePerDay = true; });
        }
    }
}