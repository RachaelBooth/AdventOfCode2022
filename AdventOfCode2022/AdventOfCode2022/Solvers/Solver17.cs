using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver17 : BaseSolver<long>
    {
        private List<char> jetPattern;

        const char Floor = '-';
        const char Wall = '|';
        const char Rock = '@';
        const char Empty = '.';

        private List<HashSet<(int x, int y)>> rockTypes;

        public Solver17()
        {
            jetPattern = InputReader<char>().ReadInputAsSingleLineByChar();
            rockTypes = new List<HashSet<(int x, int y)>>();
            rockTypes.Add(new HashSet<(int x, int y)> { (0, 0), (1, 0), (2, 0), (3, 0) });
            rockTypes.Add(new HashSet<(int x, int y)> { (1, 0), (0, 1), (1, 1), (2, 1), (1, 2) });
            rockTypes.Add(new HashSet<(int x, int y)> { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) });
            rockTypes.Add(new HashSet<(int x, int y)> { (0, 0), (0, 1), (0, 2), (0, 3) });
            rockTypes.Add(new HashSet<(int x, int y)> { (0, 0), (0, 1), (1, 0), (1, 1) });
        }

        protected override long Solve1()
        {
            var chamber = new GridMap<char>(((long x, long y) location) =>
            {
                if (location.y == 0)
                {
                    return Floor;
                }

                if (location.x == 0 || location.x == 8)
                {
                    return Wall;
                }

                return Empty;
            });

            // Eww, but it makes it easier
            var t = 1;
            while (t <= 7)
            {
                chamber.Set((t, 0), Floor);
                t++;
            }

            var rocks = 0;
            var jet = 0;
            var seen = new List<(ValueCollection<(long x, long y)> floorShape, long rocks, int nextJetIndex, long height)> { (new ValueCollection<(long x, long y)>((1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0)), 0, 0, 0) };
            while (rocks < 2022)
            {
                var rock = rockTypes[rocks % rockTypes.Count];
                var rockZero = ((long x, long y))(3, chamber.LocationsWhereValueMatches(v => v == Rock || v == Floor).Max(l => l.y) + 4);
                var isAtRest = false;
                while (!isAtRest)
                {
                    var horizontalMove = jetPattern[jet] == '<' ? -1 : 1;
                    if (rock.Any(r => chamber.ReadWithDefault(rockZero.Plus(r).Plus((horizontalMove, 0))) != Empty))
                    {
                        horizontalMove = 0;
                    }

                    rockZero = rockZero.Plus((horizontalMove, 0));

                    if (rock.Any(r => chamber.ReadWithDefault(rockZero.Plus(r).Plus((0, -1))) != Empty))
                    {
                        isAtRest = true;
                        foreach (var location in rock)
                        {
                            chamber.Set(rockZero.Plus(location), Rock);
                        }
                    }
                    else
                    {
                        rockZero = rockZero.Plus((0, -1));
                    }

                    jet = (jet + 1) % jetPattern.Count;
                }

                var height = chamber.LocationsWhereValueMatches(v => v == Rock || v == Floor).Max(l => l.y);
                var floorShape = new ValueCollection<(long x, long y)>();
                var x = 1;
                while (x <= 7)
                {
                    var highestInColumn = chamber.LocationsWhere(l => l.x == x, v => v == Rock || v == Floor).Max(l => l.y);
                    floorShape.Add((x, highestInColumn - height));
                    x++;
                }

                rocks++;

                var previous = seen.FindIndex(s => s.floorShape == floorShape && s.nextJetIndex == jet && s.rocks % rockTypes.Count == rocks % rockTypes.Count);
                if (previous != -1)
                {
                    // Have a loop

                    // integer - rounds towards zero in the division
                    var remainingFullLoops = (2022 - rocks) / (seen.Count - previous);
                    var remainder = (2022 - rocks) % (seen.Count - previous);

                    var heightAdded = remainingFullLoops * (height - seen[previous].height) + (seen[previous + remainder].height - seen[previous].height);
                    var finalHeight = height + heightAdded;
                    return finalHeight;
                }
                else
                {
                    seen.Add((floorShape, rocks, jet, height));
                }
            }

            return chamber.LocationsWhereValueMatches(v => v == Rock || v == Floor).Max(l => l.y);
        }

        protected override long Solve2()
        {
            var chamber = new GridMap<char>(((long x, long y) location) =>
            {
                if (location.y == 0)
                {
                    return Floor;
                }

                if (location.x == 0 || location.x == 8)
                {
                    return Wall;
                }

                return Empty;
            });

            // Eww, but it makes it easier
            var t = 1;
            while (t <= 7)
            {
                chamber.Set((t, 0), Floor);
                t++;
            }

            long rocks = 0;
            var jet = 0;
            var seen = new List<(ValueCollection<(long x, long y)> floorShape, long rocks, int nextJetIndex, long height)> { (new ValueCollection<(long x, long y)>((1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0)), 0, 0, 0) };
            while (rocks < 1000000000000)
            {
                var rock = rockTypes[(int) (rocks % rockTypes.Count)];
                var rockZero = ((long x, long y))(3, chamber.LocationsWhereValueMatches(v => v == Rock || v == Floor).Max(l => l.y) + 4);
                var isAtRest = false;
                while (!isAtRest)
                {
                    var horizontalMove = jetPattern[jet] == '<' ? -1 : 1;
                    if (rock.Any(r => chamber.ReadWithDefault(rockZero.Plus(r).Plus((horizontalMove, 0))) != Empty))
                    {
                        horizontalMove = 0;
                    }

                    rockZero = rockZero.Plus((horizontalMove, 0));

                    if (rock.Any(r => chamber.ReadWithDefault(rockZero.Plus(r).Plus((0, -1))) != Empty))
                    {
                        isAtRest = true;
                        foreach (var location in rock)
                        {
                            chamber.Set(rockZero.Plus(location), Rock);
                        }
                    }
                    else
                    {
                        rockZero = rockZero.Plus((0, -1));
                    }

                    jet = (jet + 1) % jetPattern.Count;
                }

                var height = chamber.LocationsWhereValueMatches(v => v == Rock || v == Floor).Max(l => l.y);
                var floorShape = new ValueCollection<(long x, long y)>();
                var x = 1;
                while (x <= 7)
                {
                    var highestInColumn = chamber.LocationsWhere(l => l.x == x, v => v == Rock || v == Floor).Max(l => l.y);
                    floorShape.Add((x, highestInColumn - height));
                    x++;
                }

                rocks++;

                var previous = seen.FindIndex(s => s.floorShape == floorShape && s.nextJetIndex == jet && s.rocks % rockTypes.Count == rocks % rockTypes.Count);
                if (previous != -1)
                {
                    // Have a loop

                    var d = Math.DivRem(1000000000000 - rocks, seen.Count - previous);
                    var remainingFullLoops = d.Quotient;
                    var remainder = (int) d.Remainder;

                    var heightAdded = remainingFullLoops * (height - seen[previous].height) + (seen[previous + remainder].height - seen[previous].height);
                    var finalHeight = height + heightAdded;
                    return finalHeight;
                }
                else
                {
                    seen.Add((floorShape, rocks, jet, height));
                }
            }

            return chamber.LocationsWhereValueMatches(v => v == Rock || v == Floor).Max(l => l.y);
        }
    }
}
