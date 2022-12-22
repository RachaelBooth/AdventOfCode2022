using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver22 : BaseSolver<long>
    {
        private GridMap<char> map;
        private string path;

        // Could use an enum but this is a bit easier for reasons
        const char NonExistant = ' ';
        const char Open = '.';
        const char Wall = '#';

        const char Left = '<';
        const char Right = '>';
        const char Up = '^';
        const char Down = 'v';

        public Solver22()
        {
            var read = InputReader<char, string>().ReadAsGridThenLines(c => c);
            map = new GridMap<char>(read.Item1, NonExistant);
            path = read.Item2[0];
        }

        protected override long Solve1()
        {
            (long x, long y) start = (map.LocationsWhere(l => l.y == 0, v => v == Open).Min(l => l.x), 0);
            var bounds = map.Bounds();
            var facing = Right;
            var p = 0;
            var current = start;
            while (p < path.Length)
            {
                var c = path[p];
                if (char.IsLetter(c))
                {
                    facing = Turn(facing, c);
                }
                else
                {
                    var section = $"{c}";
                    while (p + 1 < path.Length && char.IsDigit(path[p + 1]))
                    {
                        p++;
                        section += path[p];
                    }
                    var distance = int.Parse(section);
                    var step = UnitMoveVector(facing);
                    var s = 0;
                    while (s < distance)
                    {
                        s++;
                        var next = current.Plus(step);

                        if (map.ReadWithDefault(next) == NonExistant)
                        {
                            // Wrap
                            // Should make this nicer :shrug:
                            next = facing switch
                            {
                                Left => (bounds.maxX, next.y),
                                Right => (bounds.minX, next.y),
                                Up => (next.x, bounds.maxY),
                                Down => (next.x, bounds.minY),
                                _ => throw new Exception("whoops")
                            };

                            while (map.ReadWithDefault(next) == NonExistant)
                            {
                                next = next.Plus(step);
                            }
                        }

                        if (map.ReadWithDefault(next) == Open)
                        {
                            current = next;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                p++;
            }

            return 1000 * (current.y + 1) + 4 * (current.x + 1) + FacingToInt(facing);
        }

        private static int FacingToInt(char facing)
        {
            return facing switch
            {
                Right => 0,
                Down => 1,
                Left => 2,
                Up => 3,
                _ => throw new Exception("ah")
            };
        }

        private static (long x, long y) UnitMoveVector(char facing)
        {
            return facing switch
            {
                Left => (-1, 0),
                Right => (1, 0),
                Up => (0, -1),
                Down => (0, 1),
                _ => throw new Exception("oh dear")
            };
        }

        private static char Turn(char current, char instruction)
        {
            if (instruction == 'R')
            {
                return current switch
                {
                    Left => Up,
                    Up => Right,
                    Right => Down,
                    Down => Left,
                    _ => throw new Exception("invalid current")
                };
            }

            if (instruction == 'L')
            {
                return current switch
                {
                    Left => Down,
                    Down => Right,
                    Right => Up,
                    Up => Left,
                    _ => throw new Exception("invalid current")
                };
            }

            throw new Exception("invalid instruction");
        }

        protected override long Solve2()
        {
            (long x, long y) start = (map.LocationsWhere(l => l.y == 0, v => v == Open).Min(l => l.x), 0);
            var bounds = map.Bounds();
            var facing = Right;
            var p = 0;
            var current = start;
            while (p < path.Length)
            {
                var c = path[p];
                if (char.IsLetter(c))
                {
                    facing = Turn(facing, c);
                }
                else
                {
                    var section = $"{c}";
                    while (p + 1 < path.Length && char.IsDigit(path[p + 1]))
                    {
                        p++;
                        section += path[p];
                    }
                    var distance = int.Parse(section);
                    var s = 0;
                    while (s < distance)
                    {
                        s++;
                        var step = UnitMoveVector(facing);
                        var next = current.Plus(step);
                        var nextFacing = facing;

                        if (map.ReadWithDefault(next) == NonExistant)
                        {
                            // Wrap
                            // Makes use of looking at the shape of the input...
                            if (facing == Left)
                            {
                                if (next.y < 0)
                                {
                                    throw new Exception("1a");
                                }
                                else if (next.y < 50)
                                {
                                    nextFacing = Right;
                                    next = (0, 149 - next.y);
                                }
                                else if (next.y < 100)
                                {
                                    nextFacing = Down;
                                    next = (next.y - 50, 100);
                                }
                                else if (next.y < 150)
                                {
                                    nextFacing = Right;
                                    next = (50, 149 - next.y);
                                }
                                else if (next.y < 200)
                                {
                                    nextFacing = Down;
                                    next = (next.y - 100, 0);
                                }
                                else
                                {
                                    throw new Exception("1");
                                }
                            }
                            else if (facing == Up)
                            {
                                if (next.x < 0)
                                {
                                    throw new Exception("2a");
                                }
                                else if (next.x < 50)
                                {
                                    nextFacing = Right;
                                    next = (50, next.x + 50);
                                }
                                else if (next.x < 100)
                                {
                                    nextFacing = Right;
                                    next = (0, next.x + 100);
                                }
                                else if (next.x < 150)
                                {
                                    nextFacing = Up;
                                    next = (next.x - 100, 199);
                                }
                                else
                                {
                                    throw new Exception("2");
                                }
                            }
                            else if (facing == Right)
                            {
                                if (next.y < 0)
                                {
                                    throw new Exception("3a");
                                }
                                else if (next.y < 50)
                                {
                                    nextFacing = Left;
                                    next = (99, 149 - next.y);
                                }
                                else if (next.y < 100)
                                {
                                    nextFacing = Up;
                                    next = (next.y + 50, 49);
                                }
                                else if (next.y < 150)
                                {
                                    nextFacing = Left;
                                    next = (149, 149 - next.y);
                                }
                                else if (next.y < 200)
                                {
                                    nextFacing = Up;
                                    next = (next.y - 100, 149);
                                }
                                else
                                {
                                    throw new Exception("3");
                                }
                            }
                            else if (facing == Down)
                            {
                                if (next.x < 0)
                                {
                                    throw new Exception("4a");
                                }
                                else if (next.x < 50)
                                {
                                    nextFacing = Down;
                                    next = (next.x + 100, 0);
                                }
                                else if (next.x < 100)
                                {
                                    nextFacing = Left;
                                    next = (49, next.x + 100);
                                }
                                else if (next.x < 150)
                                {
                                    nextFacing = Left;
                                    next = (99, next.x - 50);
                                }
                                else
                                {
                                    throw new Exception("4");
                                }
                            }
                            else
                            {
                                throw new Exception("How?");
                            }
                        }

                        if (map.ReadWithDefault(next) == NonExistant)
                        {
                            throw new Exception("whoops");
                        }

                        if (map.ReadWithDefault(next) == Open)
                        {
                            current = next;
                            facing = nextFacing;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                p++;
            }

            return 1000 * (current.y + 1) + 4 * (current.x + 1) + FacingToInt(facing);
        }
    }
}
