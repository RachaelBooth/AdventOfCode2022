using System.Collections.Generic;
using System.Linq;

namespace AoCBase.Vectors
{
    public static class FourDimensionalTupleExtensions
    {
        public static (int x, int y, int z, int w) Plus(this (int x, int y, int z, int w) location, (int x, int y, int z, int w) summand)
        {
            return (location.x + summand.x, location.y + summand.y, location.z + summand.z, location.w + summand.w);
        }

        public static (int x, int y, int z, int w) Minus(this (int x, int y, int z, int w) location, (int x, int y, int z, int w) subtrahend)
        {
            return location.Plus(subtrahend.Times(-1));
        }

        public static (int x, int y, int z, int w) Times(this (int x, int y, int z, int w) location, (int x, int y, int z, int w) multiplicand)
        {
            return (location.x * multiplicand.x, location.y * multiplicand.y, location.z * multiplicand.z, location.w * multiplicand.w);
        }

        public static (int x, int y, int z, int w) Times(this (int x, int y, int z, int w) location, int multiplicand)
        {
            return location.Times((multiplicand, multiplicand, multiplicand, multiplicand));
        }

        public static IEnumerable<(int x, int y, int z, int w)> NeighbouringLocations(this (int x, int y, int z, int w) location)
        {
            return (location.x, location.y, location.z).NeighbouringLocations()
                .Select(l => (l.x, l.y, l.z, location.w))
                .SelectMany(
                    l => new List<(int x, int y, int z, int w)> { l, l.Plus((0, 0, 0, 1)), l.Minus((0, 0, 0, 1)) })
                .Append(location.Plus((0, 0, 0, 1)))
                .Append(location.Minus((0, 0, 0, 1)));
        }
    }
}
