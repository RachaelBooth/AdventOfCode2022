using AoCBase;
using System.Numerics;

namespace AdventOfCode2022.Solvers
{
    internal class Solver11 : BaseSolver<BigInteger>
    {
        protected override BigInteger Solve1()
        {
            var monkeys = InputReader<Monkey>().ReadInputAsLineGroups().ToList();
            // N.B. As it happens monkey keys are in order so will automatically match the index here
            var rounds = 0;
            while (rounds < 20)
            {
                foreach (var monkey in monkeys)
                {
                    while (monkey.HasItems())
                    {
                        var (recipient, worryLevel) = monkey.PassNextItem();
                        monkeys[recipient].RecieveItem(worryLevel);
                    }
                }
                rounds++;
            }

            var mostActive = monkeys.Select(m => m.InspectItemCount()).OrderByDescending(i => i).ToList();
            var monkeyBusiness = mostActive[0] * mostActive[1];
            return monkeyBusiness;
        }

        protected override BigInteger Solve2()
        {
            var monkeys = InputReader<Monkey>().ReadInputAsLineGroups().ToList();
            var lcm = monkeys.Select(m => m.tester).Aggregate((curr, next) => curr * next);
            // N.B. As it happens monkey keys are in order so will automatically match the index here
            var rounds = 0;
            while (rounds < 10000)
            {
                foreach (var monkey in monkeys)
                {
                    while (monkey.HasItems())
                    {
                        var (recipient, worryLevel) = monkey.PassNextItemTwo(lcm);
                        monkeys[recipient].RecieveItem(worryLevel);
                    }
                }
                rounds++;
            }

            var mostActive = monkeys.Select(m => m.InspectItemCount()).OrderByDescending(i => i).ToList();
            var monkeyBusiness = mostActive[0] * mostActive[1];
            return monkeyBusiness;
        }

        private class Monkey
        {
            public int Key;
            private Queue<long> items;
            private Func<long, long> operation;
            public int tester;
            private int trueRecipient;
            private int falseRecipient;
            private BigInteger inspectItemCount;

            public Monkey(int key, IEnumerable<long> startingItems, Func<long, long> operation, int tester, int trueRecipient, int falseRecipient)
            {
                Key = key;
                items = new Queue<long>();
                foreach (var item in startingItems)
                {
                    items.Enqueue(item);
                }
                this.operation = operation;
                this.tester = tester;
                this.trueRecipient = trueRecipient;
                this.falseRecipient = falseRecipient;
                inspectItemCount = 0;
            }

            public static Monkey Parse(List<string> lines)
            {
                var key = int.Parse(lines[0].Split(' ')[1].Trim(':'));
                var items = lines[1].Split(':')[1].Trim().Split(", ").Select(i => long.Parse(i));
                var operationParts = lines[2].Split('=')[1].Trim().Split(' ');
                // N.B. All * or +
                var operation = (Func<long, long>) (operationParts[1] == "*" ? ((long l) => l * (operationParts[2] == "old" ? l : int.Parse(operationParts[2]))) : ((long l) => l + (operationParts[2] == "old" ? l : int.Parse(operationParts[2]))));
                // N.B. All "if divisible by x throw to y else throw to z"
                var d = int.Parse(lines[3].Trim().Split(' ')[3]);
                var t = int.Parse(lines[4].Trim().Split(' ')[5]);
                var f = int.Parse(lines[5].Trim().Split(' ')[5]);
                return new Monkey(key, items, operation, d, t, f);
            }

            public bool HasItems()
            {
                return items.Any();
            }

            public (int recipient, long worryLevel) PassNextItem()
            {
                var item = items.Dequeue();
                inspectItemCount++;
                var worryLevel = item;
                worryLevel = operation(worryLevel);
                // Rounds towards 0 - same as rounding down for +ve, which these will always be
                worryLevel = worryLevel / 3;
                return (Test(worryLevel), worryLevel);
            }

            public (int recipient, long worryLevel) PassNextItemTwo(long lcm)
            {
                // Observation: We only care about divisibility of the worryLevel, notably by all the tester values.
                // So we can keep the worryLevel modulo the LCM of the tester values
                // All the tester values are prime, so can just multiply for that.

                var item = items.Dequeue();
                inspectItemCount++;
                var worryLevel = item;
                worryLevel = operation(worryLevel) % lcm;
                return (Test(worryLevel), worryLevel);
            }

            private int Test(long worryLevel)
            {
                if (worryLevel % tester == 0)
                {
                    return trueRecipient;
                }

                return falseRecipient;
            }

            public void RecieveItem(long worryLevel)
            {
                items.Enqueue(worryLevel);
            }

            public BigInteger InspectItemCount()
            {
                return inspectItemCount;
            }
        }
    }
}
