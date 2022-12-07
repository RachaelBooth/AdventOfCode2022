using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    public class Solver04 : BaseSolver<int>
    {
        private List<ElfPair> pairs;

        public Solver04()
        {
            pairs = InputReader<ElfPair>().ReadInputAsLines().ToList();
        }

        protected override int Solve1()
        {
            return pairs.Count(Envelops);
        }

        private static bool Envelops(ElfPair pair)
        {
            return (pair.First.min >= pair.Second.min && pair.First.max <= pair.Second.max) 
                || (pair.Second.min >= pair.First.min && pair.Second.max <= pair.First.max);
        }

        protected override int Solve2()
        {
            return pairs.Count(Overlaps);
        }

        private static bool Overlaps(ElfPair pair)
        {
            return pair.First.max >= pair.Second.min && pair.Second.max >= pair.First.min;
        }

        private class ElfPair
        {
            public (int min, int max) First;
            public (int min, int max) Second;

            public ElfPair (int firstMin, int firstMax, int secondMin, int secondMax)
            {
                First = (firstMin, firstMax);
                Second = (secondMin, secondMax);
            }

            public static ElfPair Parse(string line)
            {
                var parts = line.Split(',', '-').Select(p => int.Parse(p)).ToList();
                return new ElfPair(parts[0], parts[1], parts[2], parts[3]);
            }

        }
    }
}
