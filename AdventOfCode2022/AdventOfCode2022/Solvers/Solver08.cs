using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver08 : BaseSolver<int>
    {
        private GridMap<int> trees;

        public Solver08()
        {
            trees = new GridMap<int>(InputReader().Parse2DimensionalGrid(c => int.Parse(c.ToString())));
        }

        protected override int Solve1()
        {
            var visibleFromEdgeCount = 0;
            foreach (var tree in trees.Keys())
            {
                if (!FindEqualOrTallerTreeInDirection(tree, (1, 0)).found || !FindEqualOrTallerTreeInDirection(tree, (-1, 0)).found || !FindEqualOrTallerTreeInDirection(tree, (0, 1)).found || !FindEqualOrTallerTreeInDirection(tree, (0, -1)).found)
                {
                    visibleFromEdgeCount++;
                }
            }
            return visibleFromEdgeCount;
        }

        protected override int Solve2()
        {
            var best = 0;
            foreach (var tree in trees.Keys())
            {
                var score = FindScenicScore(tree);
                if (score > best)
                {
                    best = score;
                }
            }
            return best;
        }

        private (bool found, int stepCount) FindEqualOrTallerTreeInDirection((long x, long y) tree, (int x, int y) direction)
        {
            var height = trees.ReadWithDefault(tree);
            var step = 1;
            while (trees.ContainsKey(tree.Plus(direction.Times(step))))
            {
                if (trees.ReadWithDefault(tree.Plus(direction.Times(step))) >= height)
                {
                    return (true, step);
                }
                step++;
            }
            return (false, step - 1);
        }

        private int FindScenicScore((long x, long y) tree)
        {
            return FindEqualOrTallerTreeInDirection(tree, (1, 0)).stepCount * FindEqualOrTallerTreeInDirection(tree, (-1, 0)).stepCount * FindEqualOrTallerTreeInDirection(tree, (0, 1)).stepCount * FindEqualOrTallerTreeInDirection(tree, (0, -1)).stepCount;
        }
    }
}
