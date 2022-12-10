using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver09 : BaseSolver<int>
    {
        private List<Move> Moves;

        public Solver09()
        {
            this.Moves = InputReader<Move>().ReadInputAsLines().ToList();
        }

        protected override int Solve1()
        {
            var current = new RopeLocation((0, 0), (0, 0));
            var tailVisited = new HashSet<(int, int)>();
            tailVisited.Add(current.Tail);
            foreach (var move in Moves)
            {
                var i = 0;
                while (i < move.Distance)
                {
                    current = current.MoveOnce(move.Direction);
                    tailVisited.Add(current.Tail);
                    i++;
                }
            }
           // var test = new GridMap<char>(tailVisited, '#', '.');
           // test.Draw();
            return tailVisited.Count;
        }

        protected override int Solve2()
        {
            var current = new LongRopeLocation();
            var tailVisited = new HashSet<(int, int)>();
            tailVisited.Add(current.Knot9);
            foreach (var move in Moves)
            {
                var i = 0;
                while (i < move.Distance)
                {
                    current = current.MoveOnce(move.Direction);
                    tailVisited.Add(current.Knot9);
                    i++;
                }
            }
            return tailVisited.Count();
        }

        private class LongRopeLocation
        {
            public (int x, int y) Head;
            public (int x, int y) Knot1;
            public (int x, int y) Knot2;
            public (int x, int y) Knot3;
            public (int x, int y) Knot4;
            public (int x, int y) Knot5;
            public (int x, int y) Knot6;
            public (int x, int y) Knot7;
            public (int x, int y) Knot8;
            public (int x, int y) Knot9;

            public LongRopeLocation()
            {
                Head = (0, 0);
                Knot1 = (0, 0);
                Knot2 = (0, 0);
                Knot3 = (0, 0);
                Knot4 = (0, 0);
                Knot5 = (0, 0);
                Knot6 = (0, 0);
                Knot7 = (0, 0);
                Knot8 = (0, 0);
                Knot9 = (0, 0);
            }

            public LongRopeLocation((int x, int y) head, (int x, int y) one, (int x, int y) two, (int x, int y) three, (int x, int y) four, (int x, int y) five, (int x, int y) six, (int x, int y) seven, (int x, int y) eight, (int x, int y) nine)
            {
                Head = head;
                Knot1 = one;
                Knot2 = two;
                Knot3 = three;
                Knot4 = four;
                Knot5 = five;
                Knot6 = six;
                Knot7 = seven;
                Knot8 = eight;
                Knot9 = nine;
            }

            public LongRopeLocation MoveOnce(string direction)
            {
                var directionVector = direction switch
                {
                    "D" => (0, 1),
                    "U" => (0, -1),
                    "R" => (1, 0),
                    "L" => (-1, 0),
                    _ => throw new Exception("Whoops")
                };

                var newHead = Head.Plus(directionVector);
                var one = TrailingLocationAfterMotion(newHead, Knot1);
                var two = TrailingLocationAfterMotion(one, Knot2);
                var three = TrailingLocationAfterMotion(two, Knot3);
                var four = TrailingLocationAfterMotion(three, Knot4);
                var five = TrailingLocationAfterMotion(four, Knot5);
                var six = TrailingLocationAfterMotion(five, Knot6);
                var seven = TrailingLocationAfterMotion(six, Knot7);
                var eight = TrailingLocationAfterMotion(seven, Knot8);
                var nine = TrailingLocationAfterMotion(eight, Knot9);

                return new LongRopeLocation(newHead, one, two, three, four, five, six, seven, eight, nine);
            }

            private static (int x, int y) TrailingLocationAfterMotion((int x, int y) newPreceedingKnotLocation, (int x, int y) oldTrailingKnotLocation)
            {
                if (oldTrailingKnotLocation.IsWithinSquareRadius(newPreceedingKnotLocation, 1))
                {
                    return oldTrailingKnotLocation;
                }

                if (oldTrailingKnotLocation.x == newPreceedingKnotLocation.x)
                {
                    return oldTrailingKnotLocation.Plus((0, newPreceedingKnotLocation.y > oldTrailingKnotLocation.y ? 1 : -1));
                }

                if (oldTrailingKnotLocation.y == newPreceedingKnotLocation.y)
                {
                    return oldTrailingKnotLocation.Plus((newPreceedingKnotLocation.x > oldTrailingKnotLocation.x ? 1 : -1, 0));
                }

                return oldTrailingKnotLocation.Plus((newPreceedingKnotLocation.x > oldTrailingKnotLocation.x ? 1 : -1, newPreceedingKnotLocation.y > oldTrailingKnotLocation.y ? 1 : -1));
            }

        }

        private class RopeLocation
        {
            public (int x, int y) Head;
            public (int x, int y) Tail;

            public RopeLocation((int x, int y) head, (int x, int y) tail)
            {
                Head = head;
                Tail = tail;
            }

            public RopeLocation MoveOnce(string direction)
            {
                var directionVector = direction switch
                {
                    "D" => (0, 1),
                    "U" => (0, -1),
                    "R" => (1, 0),
                    "L" => (-1, 0),
                    _ => throw new Exception("Whoops")
                };

                var newHead = Head.Plus(directionVector);

                if (Tail.IsWithinSquareRadius(newHead, 1))
                {
                    return new RopeLocation(newHead, Tail);
                }

                if (Tail.x == newHead.x)
                {
                    return new RopeLocation(newHead, Tail.Plus((0, newHead.y > Tail.y ? 1 : -1)));
                }

                if (Tail.y == newHead.y)
                {
                    return new RopeLocation(newHead, Tail.Plus((newHead.x > Tail.x ? 1 : -1, 0)));
                }

                return new RopeLocation(newHead, Tail.Plus((newHead.x > Tail.x ? 1 : -1, newHead.y > Tail.y ? 1 : -1)));
            }
        }

        private class Move
        {
            public string Direction;
            public int Distance;

            public Move(string direction, int distance)
            {
                Direction = direction;
                Distance = distance;
            }

            public static Move Parse(string line)
            {
                var parts = line.Split(' ');
                return new Move(parts[0], int.Parse(parts[1]));
            }
        }
    }
}
