using AoCBase;

namespace AdventOfCode2022.Solvers
{
    internal class Solver10 : BaseSolver<long>
    {
        private List<Instruction> programme;

        public Solver10()
        {
            programme = InputReader<Instruction>().ReadInputAsLines().ToList();
        }

        protected override long Solve1()
        {
            long X = 1;
            var cycles = 1;
            long signalStrengthSum = 0;
            foreach (var instruction in programme)
            {
                var results = instruction.Run(X);
                foreach (var result in results)
                {
                    cycles++;
                    X = result;
                    if (cycles % 40 == 20)
                    {
                        signalStrengthSum += (cycles * X);
                    }
                }
                
                if (cycles >= 220)
                {
                    // N.B. Max in one instruction in 2, so only checking here is fine.
                    break;
                }
            }
            return signalStrengthSum;
        }

        protected override long Solve2()
        {
            var output = new GridMap<char>(new Dictionary<(int x, int y), char>(), '.');
            long X = 1;
            var cycles = 1;
            output.Set((0, 0), '#');
            var y = 0;
            foreach (var instruction in programme)
            {
                var results = instruction.Run(X);
                foreach (var result in results)
                {
                    cycles++;
                    X = result;
                    var drawX = (cycles - 1) % 40;
                    output.Set((drawX, y), Math.Abs(X - drawX) <= 1 ? '#' : '.');
                    if (cycles % 40 == 0)
                    {
                        y++;
                    }
                }
            }
            output.Draw();
            return 0;
        }

        private abstract class Instruction
        {
            public static Instruction Parse(string line)
            {
                var parts = line.Split(' ');
                if (parts[0] == "noop")
                {
                    return new NoOp();
                }

                return new Addx(int.Parse(parts[1]));
            }

            public abstract long[] Run(long X);
        }

        private class NoOp : Instruction
        {
            public override long[] Run(long X)
            {
                return new[] { X };
            }
        }

        private class Addx : Instruction
        {
            public int summand;

            public Addx(int summand)
            {
                this.summand = summand;
            }

            public override long[] Run(long X)
            {
                return new[] { X, X + summand };
            }
        }
    }
}
