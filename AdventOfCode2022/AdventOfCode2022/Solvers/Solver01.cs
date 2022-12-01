using AoCBase;

namespace AdventOfCode2022.Solvers
{
    public class Solver01 : BaseSolver<long>
    {
        private readonly List<long> totalCaloriesCarried;

        public Solver01()
        {
            var elves = InputReader<long>().ReadInputAsGroupedLines();
            totalCaloriesCarried = elves.Select(e => e.Sum()).ToList();
        }

        protected override long Solve1()
        {
            return totalCaloriesCarried.Max();
        }

        protected override long Solve2()
        {
            var sorted = totalCaloriesCarried.OrderByDescending(l => l).ToList();
            return sorted[0] + sorted[1] + sorted[2];
        }
    }
}
