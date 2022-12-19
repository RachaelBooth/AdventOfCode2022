using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver18 : BaseSolver<int>
    {
        private HashSet<(int x, int y, int z)> cubes;

        public Solver18()
        {
            cubes = new HashSet<(int x, int y, int z)>();
            foreach (var line in InputReader<string>().ReadInputAsLines())
            {
                var parts = line.Split(',').Select(p => int.Parse(p)).ToList();
                cubes.Add((parts[0], parts[1], parts[2]));
            }
        }

        protected override int Solve1()
        {
            var exposedFaces = 0;
            foreach (var cube in cubes)
            {
                exposedFaces += FaceNeighbours(cube).Count(f => !cubes.Contains(f));
            }
            return exposedFaces;
        }

        protected override int Solve2()
        {
            var minX = cubes.Min(c => c.x) - 1;
            var minY = cubes.Min(c => c.y) - 1;
            var minZ = cubes.Min(c => c.z) - 1;

            var maxX = cubes.Max(c => c.x) + 1;
            var maxY = cubes.Max(c => c.y) + 1;
            var maxZ = cubes.Max(c => c.z) + 1;

            var steam = new HashSet<(int x, int y, int z)>();
            steam.Add((minX, minY, minZ));
            steam.Add((maxX, maxY, maxZ));
            // Both guaranteed to be actually on the outside and not cubes

            var done = false;
            while (!done)
            {
                var toAdd = new HashSet<(int x, int y, int z)>();
                foreach (var s in steam)
                {
                    foreach (var n in FaceNeighbours(s).Where(l => l.x >= minX && l.x <= maxX && l.y >= minY && l.y <= maxY && l.z >= minZ && l.z <= maxZ))
                    {
                        if (!cubes.Contains(n) && !steam.Contains(n))
                        {
                            toAdd.Add(n);
                        }
                    }
                }

                if (!toAdd.Any())
                {
                    done = true;
                }
                else
                {
                    steam.UnionWith(toAdd);
                }
            }

            var exposedToSteamFaces = 0;
            foreach (var cube in cubes)
            {
                exposedToSteamFaces += FaceNeighbours(cube).Count(f => steam.Contains(f));
            }
            return exposedToSteamFaces;
        }

        private static List<(int x, int y, int z)> FaceNeighbours((int x, int y, int z) cube)
        {
            return new List<(int x, int y, int z)>
            {
                cube.Plus((1, 0, 0)),
                cube.Plus((-1, 0, 0)),
                cube.Plus((0, 1, 0)),
                cube.Plus((0, -1, 0)),
                cube.Plus((0, 0, 1)),
                cube.Plus((0, 0, -1))
            };
        }
    }
}
