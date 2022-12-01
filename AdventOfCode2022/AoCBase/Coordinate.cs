using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCBase
{
    // TODO: Decide if this is more or less annoying than tuples (int x, int y), and either finish or remove this.
    // These are very similar to C# Vector2 and Vector3, but sometimes it's convenient to be using integers rather than floats.
    public record struct Vector2Int(int X, int Y)
    {
        public IEnumerable<Vector2Int> Neighbours(bool includeDiagonals = false)
        {
            var diffs = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(0, 1) };
            if (includeDiagonals)
            {
                diffs.Add(new Vector2Int(1, 1));
                diffs.Add(new Vector2Int(1, -1));
            }
            diffs = diffs.SelectMany(c => new Vector2Int[] { c, -c }).ToList();
            var current = this;
            return diffs.Select(d => current + d);
        }

        public Vector3Int ToCoordinate3()
        {
            return new Vector3Int(X, Y, 0);
        }

        public static Vector2Int operator +(Vector2Int lhs, Vector2Int rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y);

        public static Vector2Int operator -(Vector2Int coord) => new(-coord.X, -coord.Y);

        public static Vector2Int operator -(Vector2Int lhs, Vector2Int rhs) => lhs + (-rhs);

        public static Vector2Int operator *(Vector2Int lhs, Vector2Int rhs) => new(lhs.X * rhs.X, lhs.Y * rhs.Y);

        public static Vector2Int operator *(Vector2Int lhs, int rhs) => new(lhs.X * rhs, lhs.Y * rhs);

        public static Vector2Int operator *(int lhs, Vector2Int rhs) => rhs * lhs;
    }

    public record struct Vector3Int(int X, int Y, int Z) 
    {
        public static Vector3Int operator +(Vector3Int lhs, Vector3Int rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);

        public static Vector3Int operator -(Vector3Int coord) => new(-coord.X, -coord.Y, -coord.Z);

        public static Vector3Int operator -(Vector3Int lhs, Vector3Int rhs) => lhs + (-rhs);

        public static Vector3Int operator *(Vector3Int lhs, int rhs) => new(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);

        public static Vector3Int operator *(int lhs, Vector3Int rhs) => rhs * lhs;

        public double Magnitude()
        {
            return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        public static List<Vector3Int> GetAllOrientations(Vector3Int vector)
        {

            return new List<Vector3Int>
            {
                // face +ve X, +ve Z is up
                new(vector.X, vector.Y, vector.Z),
                // face +ve X, -ve Z is up
                new(vector.X, -vector.Y, -vector.Z),
                // face +ve X, +ve Y is up
                new(vector.X, vector.Z, -vector.Y),
                // face +ve X, -ve Y is up
                new(vector.X, -vector.Z, vector.Y),
                // face -ve X, +ve Z is up
                new(-vector.X, -vector.Y, vector.Z),
                // face -ve X, -ve Z is up
                new(-vector.X, vector.Y, -vector.Z),
                // face -ve X, +ve Y is up
                new(-vector.X, vector.Z, vector.Y),
                // face -ve X, -ve Y is up
                new(-vector.X, -vector.Z, -vector.Y),
                // face +ve Y, +ve Z is up
                new(-vector.Y, vector.X, vector.Z),
                // face +ve Y, -ve Z is up
                new(vector.Y, vector.X, -vector.Z),
                // face +ve Y, +ve X is up
                new(vector.Z, vector.X, vector.Y),
                // face +ve Y, -ve X is up
                new(-vector.Z, vector.X, -vector.Y),
                // face -ve Y, +ve Z is up
                new(vector.Y, -vector.X, vector.Z),
                // face -ve Y, -ve Z is up
                new(-vector.Y, -vector.X, -vector.Z),
                // face -ve Y, +ve X is up
                new(vector.Z, -vector.X, -vector.Y),
                // face -ve Y, -ve X is up
                new(-vector.Z, -vector.X, vector.Y),
                // face +ve Z, +ve X is up
                new(vector.Z, -vector.Y, vector.X),
                // face +ve Z, -ve X is up
                new(-vector.Z, vector.Y, vector.X),
                // face +ve Z, +ve Y is up
                new(vector.Y, vector.Z, vector.X),
                // face +ve Z, -ve Y is up
                new(-vector.Y, -vector.Z, vector.X),
                // face -ve Z, +ve X is up
                new(vector.Z, vector.Y, -vector.X),
                // face -ve Z, -ve X is up
                new(-vector.Z, -vector.Y, -vector.X),
                // face -ve Z, +ve Y is up
                new(-vector.Y, vector.Z, -vector.X),
                // face -ve Z, -ve Y is up
                new(vector.Y, -vector.Z, -vector.X)
            };
        }
    }
}
