using System;
using System.Collections.Generic;
using System.Linq;
using AoCBase.Vectors;

namespace AoCBase
{
    // TODO: Maybe set this up with vectors rather than tuple. 
    public class GridMap<U> : Map<(int x, int y), U>
    {
        public GridMap(Dictionary<(int x, int y), U> map, U defaultValue = default) : base(map, defaultValue) {}

        public GridMap(Dictionary<(int x, int y), U> map, Func<(int x, int y), Dictionary<(int x, int y), U>, U> defaultValue) : base(map, defaultValue) {}

        public GridMap(HashSet<(int x, int y)> locations, U locationValue, U defaultValue = default) : base(locations.ToDictionary(l => l, l => locationValue), defaultValue) {}

        /// <summary>
        /// Draws map to console
        /// Assumes y increases going down
        /// Assumes value type has sensible .ToString()
        /// </summary>
        /// <param name="overlayValue"></param>
        /// <param name="overlayLocations"></param>
        public void Draw(U overlayValue = default, params (int x, int y)[] overlayLocations)
        {
            var minX = _map.Keys.Min(k => k.x);
            var maxX = _map.Keys.Max(k => k.x);
            var minY = _map.Keys.Min(k => k.y);
            var maxY = _map.Keys.Max(k => k.y);

            var y = minY;
            while (y <= maxY)
            {
                var x = minX;
                while (x <= maxX)
                {
                    if (overlayLocations.Contains((x, y)))
                    {
                        Console.Write(overlayValue);
                    }
                    else
                    {
                        Console.Write(ReadWithDefault((x, y)));
                    }

                    x++;
                }
                Console.WriteLine();
                y++;
            }
        }

        public GridMap<U> Copy()
        {
            return new GridMap<U>(_map.ToDictionary(kv => kv.Key, kv => kv.Value), _defaultValue);
        }

        public int Size()
        {
            return _map.Count;
        }
    }

    public class RecursiveGridMap<U> : Map<(int x, int y, int z), U>
    {
        public RecursiveGridMap(GridMap<U> map, U defaultValue = default) : base(
            map._map.ToDictionary(kv => kv.Key.ToThreeDimensions(), kv => kv.Value),
            (t, d) => d.ContainsKey((t.x, t.y, 0)) ? d[(t.x, t.y, 0)] : defaultValue) {}
    }

    public class Map<T, U>
    {
        internal readonly Dictionary<T, U> _map;
        internal readonly Func<T, Dictionary<T, U>, U> _defaultValue;
        internal readonly Dictionary<(T start, T end), int> _paths;
        internal readonly Dictionary<(T start, T end), List<(HashSet<U> doors, int steps)>> _pathsWithDoors;
        internal readonly Dictionary<U, List<T>> _reverseMap;

        public readonly bool HasFixedDefault = false;
        public readonly U FixedDefault;

        /// <summary>
        /// Create an instance of the Map class
        /// 
        /// This will mutate your dictionary. This is fine for most AoC uses - create a clone first if you need to.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="defaultValue"></param>
        public Map(Dictionary<T, U> map, Func<T, U> defaultValue) : this(map, (t, d) => defaultValue(t)) {}

        public Map(Dictionary<T, U> map, U defaultValue = default) : this(map, t => defaultValue) 
        {
            HasFixedDefault = true;
            FixedDefault = defaultValue;
        }

        public Map(Dictionary<T, U> map, Func<T, Dictionary<T, U>, U> defaultValue)
        {
            _map = map;
            _defaultValue = defaultValue;
            _paths = new Dictionary<(T start, T end), int>();
            _pathsWithDoors = new Dictionary<(T start, T end), List<(HashSet<U> doors, int steps)>>();
            _reverseMap = new Dictionary<U, List<T>>();
        }

        public Map<T, U> Copy()
        {
            return new Map<T, U>(_map.ToDictionary(kv => kv.Key, kv => kv.Value), _defaultValue);
        }

        public void Update(Func<U, U> updateFunction)
        {
            foreach (var key in _map.Keys)
            {
                _map[key] = updateFunction(_map[key]);
            }
        }

        public void UpdateIfExists(T key, Func<U, U> updateFunction)
        {
            if (_map.ContainsKey(key))
            {
                _map[key] = updateFunction(_map[key]);
            }
        }

        public bool ContainsKey(T key)
        {
            return _map.ContainsKey(key);
        }

        /// <summary>
        /// Reads the map at location, using the default value for the map if the location is not present.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="addIfNotPresent">Whether to add the default value to the map if location is not present. Default: true</param>
        /// <returns></returns>
        public U ReadWithDefault(T location, bool addIfNotPresent = true)
        {
            if (!_map.ContainsKey(location))
            {
                if (addIfNotPresent)
                {
                    _map.Add(location, _defaultValue(location, _map));
                }
                return _defaultValue(location, _map);
            }

            return _map[location];
        }

        public void Set(T location, U value)
        {
            _map[location] = value;
        }

        public bool Matches(Map<T, U> other)
        {
            if (_map.Any(kv => !other.ReadWithDefault(kv.Key, false).Equals(kv.Value)))
            {
                return false;
            }

            return !other._map.Any(kv => !ReadWithDefault(kv.Key, false).Equals(kv.Value));
        }

        public int BestPathLength(T start, IEnumerable<T> locationsToVisit, Func<U, bool> isDoor,
            Func<T, IEnumerable<T>> potentialNeighbours, Func<U, HashSet<U>, HashSet<U>> updatePassableDoors)
        {
            // QQ implement
            return 0;
        }

        /// <summary>
        /// Finds the shortest path that visits all the given locations, starting at the given start point
        /// potentialNeighbours function must account for walls etc, and caching code assumes symmetry etc.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="locationsToVisit"></param>
        /// <param name="potentialNeighbours"></param>
        /// <returns></returns>
        public int BestPathLength(T start, IEnumerable<T> locationsToVisit, Func<T, IEnumerable<T>> potentialNeighbours)
        {
            var states = new List<(T location, HashSet<T> visited, int steps)> {(start, new HashSet<T>(), 0)};
            var locations = locationsToVisit.ToList();
            var i = 0;
            while (i < locations.Count)
            {
                var newStates = new List<(T location, HashSet<T> visited, int steps)>();
                foreach (var state in states)
                {
                    foreach (var possibleNextLocation in locations.Where(l => !state.visited.Contains(l)))
                    {
                        var bestPath = BestPathLength(state.location, possibleNextLocation, potentialNeighbours);
                        if (bestPath != -1)
                        {
                            var newVisited = new HashSet<T>(state.visited);
                            newVisited.Add(possibleNextLocation);
                            newStates.Add((possibleNextLocation, newVisited, state.steps + bestPath));
                        }
                    }
                }

                states = newStates;
                i++;
            }

            return states.Min(s => s.steps);
        }


        // TODO: Can probably tidy these two up somewhat...
        /// <summary>
        /// Returns the shortest path between start and end
        /// The potentialNeighbours function should exclude e.g. walls
        /// Caching code assumes symmetry in paths start -> end and end -> start
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="potentialNeighbours"></param>
        /// <returns></returns>
        public int BestPathLength(T start, T end, Func<T, IEnumerable<T>> potentialNeighbours)
        {
            MapBestPathBetweenPoints(start, end, potentialNeighbours);

            return _paths[(start, end)];
        }

        // Also assumes symmetry
        private void MapBestPathBetweenPoints(T start, T end, Func<T, IEnumerable<T>> potentialNeighbours)
        {
            if (_paths.ContainsKey((start, end)))
            {
                return;
            }

            var seen = new HashSet<T>();
            var edgeStates = new List<T> {start};
            var pathSteps = 0;
            while (edgeStates.Any())
            {
                pathSteps++;
                var nextStates = new List<T>();
                foreach (var state in edgeStates)
                {
                    foreach (var potential in potentialNeighbours(state))
                    {
                        if (potential.Equals(end))
                        {
                            _paths.Add((start, end), pathSteps);
                            _paths.Add((end, start), pathSteps);
                            return;
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

            _paths.Add((start, end), -1);
            _paths.Add((end, start), -1);
        }

        /// <summary>
        /// Returns the shortest path between start and end, allowing for only the given set of 'doors' to be passable
        /// The potentialNeighbours function must take into account walls/anything always impassable.
        /// The caching code assumes symmetry of paths, minimal doors are good, and that any given map will only be used with one isDoor function.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isDoor"></param>
        /// <param name="potentialNeighbours"></param>
        /// <param name="passableDoors"></param>
        /// <returns></returns>
        // TODO: Maybe make isDoor a class property
        public int BestPathLength(T start, T end, Func<U, bool> isDoor, Func<T, IEnumerable<T>> potentialNeighbours, HashSet<U> passableDoors)
        {
            MapBestPathsBetweenPoints(start, end, isDoor, potentialNeighbours);

            var options = _pathsWithDoors.GetValues((start, end)).Where(o => o.doors.IsSubsetOf(passableDoors)).ToList();
            if (!options.Any())
            {
                return -1;
            }

            return options.Min(o => o.steps);
        }

        private void MapBestPathsBetweenPoints(T start, T end, Func<U, bool> isDoor,
            Func<T, IEnumerable<T>> potentialNeighbours)
        {
            if (_pathsWithDoors.ContainsKey((start, end)))
            {
                // If so, we've already run this and don't need to do so again
                return;
            }

            var seen = new Dictionary<T, List<(HashSet<U> doors, int steps)>>();
            var edgeStates = new List<(T location, HashSet<U> doors)> { (start, new HashSet<U>()) };
            var pathSteps = 0;
            while (edgeStates.Any())
            {
                pathSteps++;
                var nextStates = new List<(T location, HashSet<U> doors)>();
                foreach (var state in edgeStates)
                {
                    foreach (var potential in potentialNeighbours(state.location))
                    {
                        var doors = new HashSet<U>(state.doors);
                        if (!seen.ContainsMatching(potential, tuple => tuple.doors.IsSubsetOf(state.doors)))
                        {
                            if (potential.Equals(end))
                            {
                                seen.AddOptions(potential, (state.doors, pathSteps));
                                _pathsWithDoors.AddOptions((start, end), (state.doors, pathSteps));
                                _pathsWithDoors.AddOptions((end, start), (state.doors, pathSteps));
                            }
                            else
                            {
                                var value = ReadWithDefault(potential);
                                if (isDoor(value))
                                {
                                    doors.Add(value);
                                }

                                seen.AddOptions(potential, (doors, pathSteps));
                                nextStates.Add((potential, doors));
                            }
                        }
                    }
                }

                edgeStates = nextStates;
            }
        }


        public HashSet<U> ValuesInSubset(IEnumerable<T> locations)
        {
            return new HashSet<U>(locations.Select(l => ReadWithDefault(l)));
        }


        public HashSet<U> ValuesInSubsetMatching(IEnumerable<T> locations, Func<U, bool> match)
        {
            return new HashSet<U>(locations.Select(l => ReadWithDefault(l)).Where(match));
        }


        public HashSet<U> ValuesInSubsetMatching(Func<T, bool> locationMatch, Func<U, bool> valueMatch)
        {
            return new HashSet<U>(_map.Where(kv => locationMatch(kv.Key) && valueMatch(kv.Value)).Select(kv => kv.Value));
        }


        public HashSet<U> ValuesMatching(Func<U, bool> match)
        {
            return new HashSet<U>(_map.Values.Where(match));
        }

        public List<U> ValuesWhere(Func<T, bool> locationMatch)
        {
            return _map.Where(kv => locationMatch(kv.Key)).Select(kv => kv.Value).ToList();
        }

        public List<T> LocationsWhere(Func<T, bool> locationMatch)
        {
            return _map.Where(kv => locationMatch(kv.Key)).Select(kv => kv.Key).ToList();
        }

        public List<T> Keys()
        {
            return _map.Keys.ToList();
        }

        /// <summary>
        /// Assumes that value is in the dictionary
        /// Maybe use FindAll instead...
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T FindFirst(U value)
        {
            return FindAll(value).First();
        }

        public List<T> FindAll(U value)
        {
            if (!_reverseMap.ContainsKey(value))
            {
                _reverseMap.Add(value, _map.Where(x => x.Value.Equals(value)).Select(x => x.Key).ToList());
            }

            return _reverseMap[value];
        }


        public IEnumerable<KeyValuePair<T, U>> Where(Func<KeyValuePair<T, U>, bool> matcher)
        {
            return _map.Where(matcher);
        }


        // QQ: Doc
        // QQ: Make more generic collection search (i.e. with function mapping location to what to add to state)
        // This is not particularly efficient.... Should reimplement with searching each set of paths between relevant items a single time
        public int ShortestPathToCollect(T startLocation, 
            Func<T, IEnumerable<T>> potentialNeighbours,
            Func<U, bool> isRelevantItem,
            Func<U, HashSet<U>, bool> isDisallowed,
            Func<HashSet<U>, bool> isComplete)
        {
            // Assumes not collecting anything at start location
            var statesSeen = new Dictionary<T, List<HashSet<U>>>();
            statesSeen.Add(startLocation, new List<HashSet<U>>());
            var endStates = new List<(T, HashSet<U>)>();
            endStates.Add((startLocation, new HashSet<U>()));
            var steps = 0;
            while (true)
            {
                steps++;
                var nextStates = new List<(T, HashSet<U>)>();
                foreach (var state in endStates)
                {
                    foreach (var potentialMove in potentialNeighbours(state.Item1))
                    {
                        var itemAtPoint = ReadWithDefault(potentialMove);
                        if (!isDisallowed(itemAtPoint, state.Item2))
                        {
                            var newState = new HashSet<U>(state.Item2);
                            if (isRelevantItem(itemAtPoint))
                            {
                                newState.Add(itemAtPoint);
                            }

                            if (isComplete(newState))
                            {
                                return steps;
                            }

                            if (statesSeen.ContainsKey(potentialMove))
                            {
                                if (!statesSeen[potentialMove].Any(s => s.IsSupersetOf(newState)))
                                {
                                    var st = statesSeen[potentialMove].Where(s => !s.IsSubsetOf(newState))
                                        .Append(newState).ToList();
                                    statesSeen[potentialMove] = st;
                                    nextStates.Add((potentialMove, newState));
                                }
                            }
                            else
                            {
                                statesSeen.Add(potentialMove, new List<HashSet<U>> {newState});
                                nextStates.Add((potentialMove, newState));
                            }
                        }
                    }
                }

                if (!nextStates.Any())
                {
                    return 0;
                }

                endStates = nextStates;
            }
        }

        public int ShortestPathToCollect<S>(S startLocations,
            Func<S, HashSet<U>, IEnumerable<S>> potentialNeighbours,
            Func<S, HashSet<U>, HashSet<U>> updateState,
            Func<S, HashSet<U>, bool> isDisallowed,
            Func<HashSet<U>, bool> isComplete)
        {
            // Assumes not collecting anything at start location
            var statesSeen = new Dictionary<S, List<HashSet<U>>>();
            statesSeen.Add(startLocations, new List<HashSet<U>>());
            var endStates = new List<(S, HashSet<U>)>();
            endStates.Add((startLocations, new HashSet<U>()));
            var steps = 0;
            while (true)
            {
                steps++;
                var nextStates = new List<(S, HashSet<U>)>();
                foreach (var state in endStates)
                {
                    foreach (var potentialMove in potentialNeighbours(state.Item1, state.Item2))
                    {
                        if (!isDisallowed(potentialMove, state.Item2))
                        {
                            var newState = updateState(potentialMove, state.Item2);

                            if (isComplete(newState))
                            {
                                return steps;
                            }

                            if (statesSeen.ContainsKey(potentialMove))
                            {
                                if (!statesSeen[potentialMove].Any(s => s.IsSupersetOf(newState)))
                                {
                                    var st = statesSeen[potentialMove].Where(s => !s.IsSubsetOf(newState))
                                        .Append(newState).ToList();
                                    statesSeen[potentialMove] = st;
                                    nextStates.Add((potentialMove, newState));
                                }
                            }
                            else
                            {
                                statesSeen.Add(potentialMove, new List<HashSet<U>> {newState});
                                nextStates.Add((potentialMove, newState));
                            }
                        }
                    }
                }

                if (!nextStates.Any())
                {
                    return 0;
                }

                endStates = nextStates;
            }
        }
    }
}
