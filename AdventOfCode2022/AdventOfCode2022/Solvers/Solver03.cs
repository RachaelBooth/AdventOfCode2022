using AoCBase;

namespace AdventOfCode2022.Solvers
{
    public class Solver03 : BaseSolver<long>
    {
        protected override long Solve1()
        {
            var rucksacks = InputReader<Rucksack>().ReadInputAsLines();
            return rucksacks.Sum(r => r.intersection.Sum(Priority));
        }

        protected override long Solve2()
        {
            var rucksackGroups = InputReader<Rucksack>().ReadInputAsBatchedLines(3);
            return rucksackGroups.Sum(group => Priority(FindBadge(group)));
        }

        private static char FindBadge(List<Rucksack> group)
        {
            var common = group[0].items;
            for (var i = 1; i < group.Count; i++)
            {
                common.IntersectWith(group[i].items);
            }
            return common.ElementAt(0);
        }

        private int Priority(char c)
        {
            return char.IsLower(c) ? c.PositionInAlphabet() : 26 + c.PositionInAlphabet();
        }

        private class Rucksack
        {
            public Dictionary<char, int> firstCompartment;
            public Dictionary<char, int> secondCompartment;

            public List<char> intersection;
            public HashSet<char> items;

            public Rucksack(string contents)
            {
                var chars = contents.ToCharArray();
                firstCompartment = chars.Take(chars.Length / 2).Aggregate(new Dictionary<char, int>(), (dict, c) => { dict.AddToValue(c, 1); return dict; });
                secondCompartment = chars.TakeLast(chars.Length / 2).Aggregate(new Dictionary<char, int>(), (dict, c) => { dict.AddToValue(c, 1); return dict; });
                intersection = firstCompartment.Keys.ToHashSet().Intersect(secondCompartment.Keys.ToHashSet()).ToList();
                items = firstCompartment.Keys.ToHashSet().Union(secondCompartment.Keys.ToHashSet()).ToHashSet();
            }

            public static Rucksack Parse(string line)
            {
                return new Rucksack(line);
            }
        }
    }
}
