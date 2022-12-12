using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver12 : BaseSolver<int>
    {
        protected override int Solve1()
        {
            var outOfRange = 'X';
            var map = new GridMap<char>(InputReader().Parse2DimensionalGrid(), outOfRange);
            var start = map.FindFirst('S');
            var end = map.FindFirst('E');
            var canStepTo = ((int x, int y) location) => location.NeighbouringLocationsWithoutDiagonals().Where(n =>
            {
                var height = map.ReadWithDefault(n, false);
                if (height == outOfRange)
                {
                    return false;
                }

                if (height == 'E')
                {
                    height = 'z';
                }

                if (height == 'S')
                {
                    height = 'a';
                }

                var currentLocationHeight = map.ReadWithDefault(location, false);

                if (currentLocationHeight == 'E')
                {
                    currentLocationHeight = 'z';
                }

                if (currentLocationHeight == 'S')
                {
                    currentLocationHeight = 'a';
                }

                return height - currentLocationHeight <= 1;
            });
            return map.BestPathLengthWithoutSymmetry(start, end, canStepTo);
        }


        // Could definitely make this more efficient - reuse above, stop looking after current max etc. But I haven't :shrug:
        // Or reverse it and find the shortest from the end to any of the starts, with opposite step condition.
        protected override int Solve2()
        {
            var outOfRange = 'X';
            var map = new GridMap<char>(InputReader().Parse2DimensionalGrid(), outOfRange);
            var end = map.FindFirst('E');
            var canStepTo = ((int x, int y) location) => location.NeighbouringLocationsWithoutDiagonals().Where(n =>
            {
                var height = map.ReadWithDefault(n, false);
                if (height == outOfRange)
                {
                    return false;
                }

                if (height == 'E')
                {
                    height = 'z';
                }

                if (height == 'S')
                {
                    height = 'a';
                }

                var currentLocationHeight = map.ReadWithDefault(location, false);

                if (currentLocationHeight == 'E')
                {
                    currentLocationHeight = 'z';
                }

                if (currentLocationHeight == 'S')
                {
                    currentLocationHeight = 'a';
                }

                return height - currentLocationHeight <= 1;
            });
            var possibleStarts = map.FindAll('a');
            possibleStarts.Add(map.FindFirst('S'));

            return possibleStarts.Select(s => map.BestPathLengthWithoutSymmetry(s, end, canStepTo)).Where(v => v != -1).Min();
        }
    }
}
