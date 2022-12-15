using AoCBase;
using AoCBase.Vectors;
using System.Numerics;

namespace AdventOfCode2022.Solvers
{
    internal class Solver15 : BaseSolver<int, BigInteger>
    {
        private List<((int x, int y) sensor, (int x, int y) closestBeacon)> data;

        public Solver15()
        {
            data = new List<((int x, int y) sensor, (int x, int y) closestBeacon)>();
            foreach (var line in InputReader<string>().ReadInputAsLines())
            {
                var parts = line.Split(": ");
                
                var sensorX = int.Parse(parts[0].Split(' ')[2].TrimEnd(',').Split('=')[1]);
                var sensorY = int.Parse(parts[0].Split(' ')[3].Split('=')[1]);
                (int x, int y) sensor = (sensorX, sensorY);

                var beaconX = int.Parse(parts[1].Split(' ')[4].TrimEnd(',').Split('=')[1]);
                var beaconY = int.Parse(parts[1].Split(' ')[5].Split('=')[1]);
                (int x, int y) beacon = (beaconX, beaconY);

                data.Add((sensor, beacon));
            }
        }

        protected override int Solve1()
        {
            var rowY = 2000000;
            var excluded = new HashSet<(int x, int y)>();
            foreach (var (sensor, closestBeacon) in data)
            {
                var distance = sensor.ManhattanDistanceFrom(closestBeacon);
                if (Math.Abs(sensor.y - rowY) <= distance)
                {
                    var remainingDistance = distance - Math.Abs(sensor.y - rowY);
                    var x = sensor.x - remainingDistance;
                    while (x <= sensor.x + remainingDistance)
                    {
                        // Are within distance from sensor to closest beacon on rowY
                        // Can't be a beacon unless we are that closest beacon
                        if (closestBeacon != (x, rowY))
                        {
                            excluded.Add((x, rowY));
                        }
                        x++;
                    }
                }
            }
            return excluded.Count;
        }

        protected override BigInteger Solve2()
        {
            var max = 4000000;
            var excluded = new HashSet<(int x, int y)>();
            foreach (var (sensor, closestBeacon) in data)
            {
                var distance = sensor.ManhattanDistanceFrom(closestBeacon);
                foreach (var location in sensor.LocationsAtManhattenDistance(distance + 1))
                {
                    if (location.x >= 0 && location.x <= max && location.y >= 0 && location.y <= max && !excluded.Contains(location))
                    {
                        var possible = true;
                        foreach (var item in data)
                        {
                            if (location.ManhattanDistanceFrom(item.sensor) <= item.closestBeacon.ManhattanDistanceFrom(item.sensor))
                            {
                                excluded.Add(location);
                                possible = false;
                                break;
                            }
                        }

                        if (possible)
                        {
                            return ((BigInteger) location.x) * 4000000 + location.y;
                        }
                    }
                }
            }
            return 0;
        }
    }
}
