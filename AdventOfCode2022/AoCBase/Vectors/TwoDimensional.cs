using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCBase.Vectors
{
    // TODO: Latest C# includes Vector2 etc: potentially move over to using those (and e.g. add neighbours methods), although need
    // to think about int v float
    public static class TwoDimensionalTupleExtensions
    {
        public static (int x, int y) Plus(this (int x, int y) location, (int x, int y) summand)
        {
            return (location.x + summand.x, location.y + summand.y);
        }

        public static (int x, int y) Minus(this (int x, int y) location, (int x, int y) subtrahend)
        {
            return location.Plus(subtrahend.Times(-1));
        }

        public static (int x, int y) Times(this (int x, int y) location, (int x, int y) multiplicand)
        {
            return (location.x * multiplicand.x, location.y * multiplicand.y);
        }

        public static (int x, int y) Times(this (int x, int y) location, int multiplicand)
        {
            return location.Times((multiplicand, multiplicand));
        }

        public static IEnumerable<(int x, int y)> NeighbouringLocations(this (int x, int y) location)
        {
            var basicDiffs = new List<(int x, int y)> { (1, 0), (0, 1), (1, 1), (1, -1) };
            return basicDiffs.SelectMany(d => new List<(int x, int y)> { location.Plus(d), location.Minus(d) });
        }

        public static IEnumerable<(int x, int y)> NeighbouringLocationsWithoutDiagonals(this (int x, int y) location)
        {
            var basicDiffs = new List<(int x, int y)> { (1, 0), (0, 1) };
            return basicDiffs.SelectMany(d => new List<(int x, int y)> { location.Plus(d), location.Minus(d) });
        }

        public static bool IsWithinSquareRadius(this (int x, int y) location, (int x, int y) other, int squareRadius = 1)
        {
            return Math.Abs(location.x - other.x) <= squareRadius && Math.Abs(location.y - other.y) <= squareRadius;
        }

        public static int ManhattanDistanceFrom(this (int x, int y) location, (int x, int y) other)
        {
            return Math.Abs(location.x - other.x) + Math.Abs(location.y - other.y);
        }

        public static IEnumerable<(int x, int y)> LocationsWithinManhattenDistance(this (int x, int y) location, int distance)
        {
            var edges = new List<(int x, int y)> { location };
            var results = new HashSet<(int x, int y)>();
            while (edges.Any())
            {
                var newEdges = new List<(int x, int y)>();
                foreach (var edge in edges)
                {
                    var neighbours = edge.NeighbouringLocationsWithoutDiagonals();
                    foreach (var neighbour in neighbours)
                    {
                        var neighbourDistance = neighbour.ManhattanDistanceFrom(location);
                        if (neighbourDistance <= distance && !results.Contains(neighbour))
                        {
                            newEdges.Add(neighbour);
                            results.Add(neighbour);
                        }
                    }
                }
                edges = newEdges;
            }
            return results;
        }

        public static IEnumerable<(int x, int y)> LocationsAtManhattenDistance(this (int x, int y) location, int distance)
        {
            if (distance <= 0)
            {
                throw new Exception("Really?");
            }

            var x = distance;
            while (x >= 0)
            {
                yield return (location.x + x, location.y + distance - x);
                yield return (location.x - x, location.y + distance - x);
                yield return (location.x + x, location.y - (distance - x));
                yield return (location.x - x, location.y - (distance - x));
                x--;
            }
        }
    }
}
