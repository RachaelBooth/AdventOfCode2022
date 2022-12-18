using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver16 : BaseSolver<int>
    {
        private Dictionary<string, int> valveFlow;
        private Dictionary<(string, string), int> shortestPaths;
        private Dictionary<string, HashSet<string>> tunnels;

        const string Start = "AA";

        // Use Map - extend that to hold step information?
        public Solver16()
        {
            valveFlow = new Dictionary<string, int>();
            shortestPaths = new Dictionary<(string, string), int>();
            tunnels = new Dictionary<string, HashSet<string>>();

            foreach (var line in InputReader<string>().ReadInputAsLines())
            {
                var parts = line.Split(';');
                var firstParts = parts[0].Split(' ');
                var valve = firstParts[1];
                var flowRate = int.Parse(firstParts[4].Split('=')[1]);
                var secondParts = parts[1].TrimStart().Split(' ');
                tunnels.Add(valve, secondParts.Skip(4).Select(v => v.TrimEnd(',')).ToHashSet());

                if (flowRate > 0)
                {
                    valveFlow.Add(valve, flowRate);
                }
            }
        }

        private int ShortestPathLength(string start, string end)
        {
            if (shortestPaths.ContainsKey((start, end)))
            {
                return shortestPaths[(start, end)];
            }

            var seen = new HashSet<string>();
            var edgeStates = new List<string> { start };
            var steps = 0;
            while (edgeStates.Any())
            {
                steps++;
                var nextStates = new List<string>();
                foreach (var state in edgeStates)
                {
                    foreach (var potential in tunnels.GetValues(state))
                    {
                        if (potential == end)
                        {
                            // Haven't assumed symmetry but they probably are to be fair
                            shortestPaths.Add((start, end), steps);
                            return steps;
                        }

                        if (!seen.Contains(potential))
                        {
                            seen.Add(potential);
                            nextStates.Add(potential);
                        }
                    }
                }

                edgeStates = nextStates;
            }

            throw new Exception("Expect connected tunnels");

        }

        protected override int Solve1()
        {
            var states = new HashSet<(string location, ValueCollection<string> opened, int timeTaken, int pressureReleased)> { (Start, new ValueCollection<string>(), 0, 0) };
            var completedMaxReleased = 0;
            var positiveFlowValveCount = valveFlow.Count;
            var maxTime = 30;

            while (states.Any())
            {
                var newStates = new HashSet<(string location, ValueCollection<string> opened, int timeTaken, int pressureReleased)>();
                foreach (var state in states)
                {
                    // Check final value if no more moves
                    var endReleased = state.pressureReleased + (maxTime - state.timeTaken) * state.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]);
                    if (endReleased > completedMaxReleased)
                    {
                        completedMaxReleased = endReleased;
                    }

                    foreach (var possibleNextLocation in valveFlow.Keys.Where(v => !state.opened.Contains(v)))
                    {
                        var bestPath = ShortestPathLength(state.location, possibleNextLocation);
                        // No benefit if wouldn't be able to open with at least 1 minute to spare
                        if (state.timeTaken + bestPath <= maxTime - 2)
                        {
                            var newOpened = state.opened.Clone();
                            newOpened.Add(possibleNextLocation);
                            var released = state.pressureReleased + (bestPath + 1) * state.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]);

                            (string location, ValueCollection<string> opened, int timeTaken, int pressureReleased) next = (possibleNextLocation, newOpened, state.timeTaken + bestPath + 1, released);

                            if (newStates.Any(s => s.location == possibleNextLocation && s.opened == newOpened))
                            {
                                var others = newStates.Where(s => s.location == possibleNextLocation && s.opened == newOpened);
                                var canDiscardNew = false;
                                foreach (var other in others)
                                {
                                    var newIsEarlier = next.timeTaken < other.timeTaken;
                                    var earlier = newIsEarlier ? next : other;
                                    var later = newIsEarlier ? other : next;

                                    // How much will have been released by the time of the later one
                                    var earlierAdjustedReleased = earlier.pressureReleased + (later.timeTaken - earlier.timeTaken) * earlier.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]);
                                    if (later.pressureReleased <= earlierAdjustedReleased)
                                    {
                                        // Can discard the later: earlier must beat it
                                        if (newIsEarlier)
                                        {
                                            newStates.Remove(other);
                                        }
                                        else
                                        {
                                            canDiscardNew = true;
                                        }
                                    }
                                }

                                if (!canDiscardNew)
                                {
                                    newStates.Add(next);
                                }
                            }
                            else
                            {
                                newStates.Add(next);
                            }
                        }
                    }
                }

                states = newStates;
            }

            return completedMaxReleased;
        }

        // 1 min 35 s for this part, but it completes so it'll do
        protected override int Solve2()
        {
            var best = 0;

            var valves = new ValueCollection<string>(valveFlow.Keys.ToHashSet());
            foreach (var subset in valves.GetSubsets().Where(s => s.Count() <= valves.Count() / 2))
            {
                var compliment = valves.Without(subset);
                var theoreticalMaxCompliment = 0;
                
                // Ewww
                compliment.ToList().OrderByDescending(v => valveFlow[v]).Select((v, index) =>
                {
                    theoreticalMaxCompliment += valveFlow[v] * (26 - index);
                    return 0;
                });

                var leftBest = BestReleaseFromSet(subset, theoreticalMaxCompliment);
                var rightBest = BestReleaseFromSet(compliment, best - leftBest);
                var released = leftBest + rightBest;
                if (released > best)
                {
                    best = released;
                }
            }

            return best;
        }

        // For part 2...
        // May return smaller than the best from this set if the abandonThreshold can't be beaten
        private int BestReleaseFromSet(ValueCollection<string> valvesToOpen, int abandonThreshold)
        {
            var maxTime = 26;
            var states = new HashSet<(string location, ValueCollection<string> opened, int timeTaken, int pressureReleased)> { (Start, new ValueCollection<string>(), 0, 0) };
            var completedMaxReleased = 0;
            var maxPressure = valvesToOpen.Aggregate(0, (curr, next) => curr + valveFlow[next]);

            while (states.Any())
            {
                var newStates = new HashSet<(string location, ValueCollection<string> opened, int timeTaken, int pressureReleased)>();
                foreach (var state in states)
                {
                    // Check final value if no more moves
                    var endReleased = state.pressureReleased + (maxTime - state.timeTaken) * state.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]);
                    if (endReleased > completedMaxReleased)
                    {
                        completedMaxReleased = endReleased;
                    }

                    foreach (var possibleNextLocation in valvesToOpen.Where(v => !state.opened.Contains(v)))
                    {
                        var bestPath = ShortestPathLength(state.location, possibleNextLocation);
                        // No benefit if wouldn't be able to open with at least 1 minute to spare
                        if (state.timeTaken + bestPath <= maxTime - 2)
                        {
                            var newOpened = state.opened.Clone();
                            newOpened.Add(possibleNextLocation);
                            var released = state.pressureReleased + (bestPath + 1) * state.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]);

                            (string location, ValueCollection<string> opened, int timeTaken, int pressureReleased) next = (possibleNextLocation, newOpened, state.timeTaken + bestPath + 1, released);

                            if (newStates.Any(s => s.location == possibleNextLocation && s.opened == newOpened))
                            {
                                var others = newStates.Where(s => s.location == possibleNextLocation && s.opened == newOpened);
                                var canDiscardNew = false;
                                foreach (var other in others)
                                {
                                    var newIsEarlier = next.timeTaken < other.timeTaken;
                                    var earlier = newIsEarlier ? next : other;
                                    var later = newIsEarlier ? other : next;

                                    // How much will have been released by the time of the later one
                                    var earlierAdjustedReleased = earlier.pressureReleased + (later.timeTaken - earlier.timeTaken) * earlier.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]);
                                    if (later.pressureReleased <= earlierAdjustedReleased)
                                    {
                                        // Can discard the later: earlier must beat it
                                        if (newIsEarlier)
                                        {
                                            newStates.Remove(other);
                                        }
                                        else
                                        {
                                            canDiscardNew = true;
                                        }
                                    }
                                }

                                if (!canDiscardNew)
                                {
                                    // Discard if it can't possibly beat the threshold
                                    if (next.pressureReleased + next.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]) + Math.Max(maxTime - next.timeTaken - 1, 0) * maxPressure > abandonThreshold)
                                    {
                                        newStates.Add(next);
                                    }
                                }
                            }
                            else
                            {
                                // Discard if it can't possibly beat the threshold
                                if (next.pressureReleased + next.opened.Aggregate(0, (curr, next) => curr + valveFlow[next]) + Math.Max(maxTime - next.timeTaken - 1, 0) * maxPressure > abandonThreshold)
                                {
                                    newStates.Add(next);
                                }
                            }
                        }
                    }
                }

                states = newStates;
            }

            return completedMaxReleased;
        }
    }
}
