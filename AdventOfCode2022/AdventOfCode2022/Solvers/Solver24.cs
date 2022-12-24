using AoCBase;
using AoCBase.Vectors;

namespace AdventOfCode2022.Solvers
{
    internal class Solver24 : BaseSolver<int>
    {
        private HashSet<(long x, long y)> leftBlizzards;
        private HashSet<(long x, long y)> rightBlizzards;
        private HashSet<(long x, long y)> upBlizzards;
        private HashSet<(long x, long y)> downBlizzards;

        private int xWrap;
        private int yWrap;

        private int xyLcm;

        private (long x, long y) Start;
        private (long x, long y) End;

        private List<(long x, long y)> basePotentialMoves;

        public Solver24()
        {
            leftBlizzards = new HashSet<(long x, long y)>();
            rightBlizzards = new HashSet<(long x, long y)>();
            upBlizzards = new HashSet<(long x, long y)>();
            downBlizzards = new HashSet<(long x, long y)>();

            var y = -1;
            foreach (var line in InputReader<string>().ReadInputAsLines())
            {
                var x = -1;
                foreach (var c in line)
                {
                    if (c == '<')
                    {
                        leftBlizzards.Add((x, y));
                    }

                    if (c == '>')
                    {
                        rightBlizzards.Add((x, y));
                    }

                    if (c == '^')
                    {
                        upBlizzards.Add((x, y));
                    }

                    if (c == 'v')
                    {
                        downBlizzards.Add((x, y));
                    }

                    x++;
                }
                xWrap = x - 1;
                y++;
            }

            yWrap = y - 1;

            // Yeah, just looking at the input and relying on that here
            Start = (0, -1);
            End = (xWrap - 1, yWrap);

            xyLcm = (xWrap * yWrap) / (int) ModularArithmetic.GreatestCommonDivisor(xWrap, yWrap);

            basePotentialMoves = new List<(long x, long y)> { (0, 0), (0, 1), (1, 0), (0, -1), (-1, 0) };
        }

        protected override int Solve1()
        {
            return ShortestPath(Start, End);
        }

        protected override int Solve2()
        {
            // Should avoid running this twice, but :shrug:
            var there = ShortestPath(Start, End);
            var back = ShortestPath(End, Start, there);
            var thereAgain = ShortestPath(Start, End, there + back);
            return there + back + thereAgain;
        }

        private int ShortestPath((long x, long y) from, (long x, long y) to, int stepOffset = 0)
        {
            var seen = new Dictionary<int, HashSet<(long x, long y)>>();
            seen.Add(0, new HashSet<(long x, long y)> { from });
            var edges = new HashSet<(long x, long y)> { from };
            var steps = 0;
            while (true)
            {
                steps++;
                var newEdges = new HashSet<(long x, long y)>();
                foreach (var state in edges)
                {
                    var potentialMoves = basePotentialMoves.Select(p => state.Plus(p));
                    foreach (var move in potentialMoves)
                    {
                        if (move == to)
                        {
                            // Always in range and not a blizzard
                            return steps;
                        }

                        if (IsInRange(move) && !IsBlizzard(move, steps + stepOffset))
                        {
                            if (!seen.GetValues(steps % xyLcm).Contains(move))
                            {
                                newEdges.Add(move);
                            }
                        }
                    }
                }
                seen.AddOptions(steps % xyLcm, newEdges.ToArray());
                edges = newEdges;

                if (!edges.Any())
                {
                    throw new Exception("Oh dear");
                }
            }
        }

        private bool IsInRange((long x, long y) location)
        {
            if (location.x < 0)
            {
                return false;
            }

            if (location.x >= xWrap)
            {
                return false;
            }

            if (location.y < 0 && location != Start)
            {
                return false;
            }

            if (location.y >= yWrap && location != End)
            {
                return false;
            }

            return true;
        }

        private bool IsBlizzard((long x, long y) location, int steps)
        {
            if (leftBlizzards.Contains(((location.x + steps) % xWrap, location.y)))
            {
                return true;
            }

            if (rightBlizzards.Contains((ModularArithmetic.NonNegativeMod(location.x - steps, xWrap), location.y)))
            {
                return true;
            }

            if (upBlizzards.Contains((location.x, (location.y + steps) % yWrap)))
            {
                return true;
            }

            if (downBlizzards.Contains((location.x, ModularArithmetic.NonNegativeMod(location.y - steps, yWrap))))
            {
                return true;
            }

            return false;
        }
    }
}
