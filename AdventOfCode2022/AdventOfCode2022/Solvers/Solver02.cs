using AoCBase;

namespace AdventOfCode2022.Solvers
{
    public class Solver02 : BaseSolver<long>
    {
        private readonly List<RoundGuide> strategyGuide;

        public Solver02()
        {
            strategyGuide = InputReader<RoundGuide>().ReadInputAsLines().ToList();
        }

        protected override long Solve1()
        {
            return strategyGuide.Sum(g => Score1(g));
        }

        protected override long Solve2()
        {
            return strategyGuide.Sum(g => Score2(g));
        }

        private int Score1(RoundGuide guide)
        {
            var yourChoice = guide.yourInstruction switch
            {
                'X' => Choice.Rock,
                'Y' => Choice.Paper,
                'Z' => Choice.Scissors,
                _ => throw new ArgumentException()
            };
            return Score(guide.opponentChoice, yourChoice);
        }

        private int Score2(RoundGuide guide)
        {
            var yourChoice = guide.yourInstruction switch
            {
                'X' => (Choice) (int) (ModularArithmetic.PositiveMod((int) guide.opponentChoice + 2, 3)),
                'Y' => guide.opponentChoice,
                'Z' => (Choice) (int) (ModularArithmetic.PositiveMod((int) guide.opponentChoice + 1, 3)),
                _ => throw new ArgumentException()
            };
            return Score(guide.opponentChoice, yourChoice);
        }

        private int Score(Choice opponent, Choice you)
        {
            return (int) you + OutcomeScore(opponent, you);
        }

        private int OutcomeScore(Choice opponent, Choice you)
        {
            if (opponent == you)
            {
                return 3;
            }

            if (ModularArithmetic.PositiveMod((int) you - 1, 3) == (int) opponent)
            {
                return 6;
            }

            return 0;
        }

        private enum Choice
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }

        private class RoundGuide
        {
            public Choice opponentChoice;
            public char yourInstruction;

            public RoundGuide(Choice opponentChoice, char yourInstruction)
            {
                this.opponentChoice = opponentChoice;
                this.yourInstruction = yourInstruction;
            }

            public static RoundGuide Parse(string line)
            {
                var parts = line.Split(' ');
                var opponentChoice = parts[0] switch
                {
                    "A" => Choice.Rock,
                    "B" => Choice.Paper,
                    "C" => Choice.Scissors,
                    _ => throw new ArgumentException(),
                };
                return new RoundGuide(opponentChoice, parts[1][0]);
            }
        }
    }
}
