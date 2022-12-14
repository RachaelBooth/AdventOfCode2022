using AoCBase;
using AoCBase.Vectors;

namespace AdventOfCode2022.Solvers
{
    internal class Solver14 : BaseSolver<int>
    {
        private GridMap<char> initialMap;

        public Solver14()
        {
            initialMap = new GridMap<char>('.');
            foreach (var line in InputReader<string>().ReadInputAsLines())
            {
                var parts = line.Split(" -> ").Select(part =>
                {
                    var p = part.Split(',');
                    var x = int.Parse(p[0]);
                    var y = int.Parse(p[1]);
                    return (x, y);
                }).ToList();
                var i = 0;
                while (i < parts.Count -1)
                {
                    var start = parts[i];
                    var end = parts[i + 1];

                    var move = end.Minus(start);

                    // For this input, either move.x or move.y will be 0. This will be (0, 1), (0, -1), (1, 0), or (-1, 0)
                    // Don't need to worry about what a unit step should be for e.g. (1, 2)
                    var step = (move.x == 0 ? 0 : move.x / Math.Abs(move.x), move.y == 0 ? 0 : move.y / Math.Abs(move.y));

                    var current = start;
                    while (current != end)
                    {
                        initialMap.Set(current, '#');
                        current = current.Plus(step);
                    }

                    initialMap.Set(end, '#');

                    i++;
                }
            }
        }

        protected override int Solve1()
        {
            var map = initialMap.Copy();
            var sandAtRest = 0;
            //  Returns true if sand has come to rest
            while (ReleaseSandUnit(map))
            {
                sandAtRest++;
            }
            return sandAtRest;
        }

        protected override int Solve2()
        {
            var floor = initialMap.Bounds().maxY + 2;
            var map = initialMap.Copy(((int x, int y) l) => l.y == floor ? '#' : '.');

            var sandAtRest = 0;
            while (map.ReadWithDefault((500, 0)) != 'o')
            {
                ReleaseSandUnit(map);
                sandAtRest++;
            }
            return sandAtRest;
        }


        // Should probably have done something clever rather than simulating from the start each time, but I didn't
        // Mutates map!
        // Returns true if sand has come to rest
        private bool ReleaseSandUnit(GridMap<char> map)
        {
            (int x, int y) sand = (500, 0);
            // To cover part 2...
            var lowestPointInMap = map.Bounds().maxY + 3;

            while (sand.y < lowestPointInMap)
            {
                var down = sand.Plus((0, 1));
                var downLeft = sand.Plus((-1, 1));
                var downRight = sand.Plus((1, 1));
                if (map.ReadWithDefault(down) == '.')
                {
                    sand = down;
                }
                else if (map.ReadWithDefault(downLeft) == '.')
                {
                    sand = downLeft;
                }
                else if (map.ReadWithDefault(downRight) == '.')
                {
                    sand = downRight;
                }
                else
                {
                    // Comes to rest
                    map.Set(sand, 'o');
                    return true;
                }
            }
            return false;
        }
    }
}
