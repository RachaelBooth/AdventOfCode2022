using System.Collections.Generic;
using System.Linq;

namespace AoCBase.Vectors
{
    public static class ThreeDimensionalTupleExtensions
    {
        public static (int x, int y, int z) Plus(this (int x, int y, int z) location, (int x, int y, int z) summand)
        {
            return (location.x + summand.x, location.y + summand.y, location.z + summand.z);
        }

        public static (int x, int y, int z) Minus(this (int x, int y, int z) location, (int x, int y, int z) subtrahend)
        {
            return location.Plus(subtrahend.Times(-1));
        }

        public static (int x, int y, int z) Times(this (int x, int y, int z) location, (int x, int y, int z) multiplicand)
        {
            return (location.x * multiplicand.x, location.y * multiplicand.y, location.z * multiplicand.z);
        }

        public static (int x, int y, int z) Times(this (int x, int y, int z) location, int multiplicand)
        {
            return location.Times((multiplicand, multiplicand, multiplicand));
        }

        public static IEnumerable<(int x, int y, int z)> NeighbouringLocations(this (int x, int y, int z) location)
        {
            return (location.x, location.y).NeighbouringLocations()
                .Select(l => (l.x, l.y, location.z))
                .SelectMany(l => new List<(int x, int y, int z)> { l, l.Plus((0, 0, 1)), l.Minus((0, 0, 1)) })
                .Append(location.Plus((0, 0, 1)))
                .Append(location.Minus((0, 0, 1)));
        }
    }
}
