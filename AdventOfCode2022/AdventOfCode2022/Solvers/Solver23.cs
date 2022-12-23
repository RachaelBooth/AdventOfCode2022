using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver23 : BaseSolver<long>
    {
        private readonly HashSet<(long x, long y)> InitialElfLocations;

        public Solver23()
        {
            InitialElfLocations = InputReader().Parse2DimensionalGrid().Where(kv => kv.Value == '#').Select(kv => kv.Key).ToHashSet();
        }

        protected override long Solve1()
        {
            var currentLocations = InitialElfLocations.ToHashSet();
            var round = 0;
            while (round < 10)
            {
                // Key = proposed new location, value is previous location
                var proposedMoves = new Dictionary<(long x, long y), (long x, long y)?>();
                foreach (var elf in currentLocations.Where(e => e.NeighbouringLocations().Any(n => currentLocations.Contains(n))))
                {
                    var proposed = GetProposedMove(round, elf, currentLocations);
                    if (proposed.HasValue)
                    {
                        if (proposedMoves.ContainsKey(proposed.Value))
                        {
                            proposedMoves[proposed.Value] = null;
                        }
                        else
                        {
                            proposedMoves.Add(proposed.Value, elf);
                        }
                    }
                }

                var afterMoves = proposedMoves.Where(kv => kv.Value.HasValue).Select(kv => kv.Key).ToHashSet();
                afterMoves.UnionWith(currentLocations.Where(e => !proposedMoves.ContainsValue(e)));

                round++;
                currentLocations = afterMoves;
            }

            var minX = currentLocations.Min(e => e.x);
            var maxX = currentLocations.Max(e => e.x);
            var minY = currentLocations.Min(e => e.y);
            var maxY = currentLocations.Max(e => e.y);

            var rectangleSize = (maxX - minX + 1) * (maxY - minY + 1);
            return rectangleSize - currentLocations.Count;
        }

        private (long x, long y)? GetProposedMove(int round, (long x, long y) elf, HashSet<(long x, long y)> currentLocations)
        {
            // North - (0, -1), also check (-1, -1), (1, -1)
            // South - (0, 1), check (-1, 1), (1, 1)
            // West - (-1, 0), check (-1, -1), (-1, 1)
            // East - (1, 0), check (1, -1), (1, 1)

            var checks = new List<((long x, long y) result, bool canPropose)>
            {
                // North
                (elf.Plus((0, -1)), !currentLocations.Contains(elf.Plus((0, -1))) && !currentLocations.Contains(elf.Plus((-1, -1))) && !currentLocations.Contains(elf.Plus((1, -1)))),
                // South
                (elf.Plus((0, 1)), !currentLocations.Contains(elf.Plus((0, 1))) && !currentLocations.Contains(elf.Plus((-1, 1))) && !currentLocations.Contains(elf.Plus((1, 1)))),
                // West
                (elf.Plus((-1, 0)), !currentLocations.Contains(elf.Plus((-1, 0))) && !currentLocations.Contains(elf.Plus((-1, -1))) && !currentLocations.Contains(elf.Plus((-1, 1)))),
                // East
                (elf.Plus((1, 0)), !currentLocations.Contains(elf.Plus((1, 0))) && !currentLocations.Contains(elf.Plus((1, -1))) && !currentLocations.Contains(elf.Plus((1, 1))))
            };

            if (checks[round % 4].canPropose)
            {
                return checks[round % 4].result;
            }

            if (checks[(round + 1) % 4].canPropose)
            {
                return checks[(round + 1) % 4].result;
            }

            if (checks[(round + 2) % 4].canPropose)
            {
                return checks[(round + 2) % 4].result;
            }

            if (checks[(round + 3) % 4].canPropose)
            {
                return checks[(round + 3) % 4].result;
            }

            return null;
        }

        protected override long Solve2()
        {
            var currentLocations = InitialElfLocations.ToHashSet();
            var done = false;
            var round = 0;
            while (!done)
            {
                // Key = proposed new location, value is previous location
                var proposedMoves = new Dictionary<(long x, long y), (long x, long y)?>();
                foreach (var elf in currentLocations.Where(e => e.NeighbouringLocations().Any(n => currentLocations.Contains(n))))
                {
                    var proposed = GetProposedMove(round, elf, currentLocations);
                    if (proposed.HasValue)
                    {
                        if (proposedMoves.ContainsKey(proposed.Value))
                        {
                            proposedMoves[proposed.Value] = null;
                        }
                        else
                        {
                            proposedMoves.Add(proposed.Value, elf);
                        }
                    }
                }

                var afterMoves = proposedMoves.Where(kv => kv.Value.HasValue).Select(kv => kv.Key).ToHashSet();
                if (!afterMoves.Any())
                {
                    done = true;
                }
                afterMoves.UnionWith(currentLocations.Where(e => !proposedMoves.ContainsValue(e)));

                round++;
                currentLocations = afterMoves;
            }

            return round;
        }
    }
}
