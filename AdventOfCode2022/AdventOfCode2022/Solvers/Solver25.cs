using AoCBase;

namespace AdventOfCode2022.Solvers
{
    internal class Solver25 : BaseSolver<string>
    {
        protected override string Solve1()
        {
            return InputReader<Snafu>().ReadInputAsLines().Aggregate((curr, next) => curr + next).ToString();
        }

        protected override string Solve2()
        {
            return "Merry Christmas!";
        }

        private class Snafu
        {
            private long value;

            public Snafu(long value)
            {
                this.value = value;
            }

            public static Snafu Parse(string line)
            {
                long value = 0;
                var i = 0;
                while (i < line.Length)
                {
                    value += (long) Math.Pow(5, i) * line[line.Length - 1 - i] switch
                    {
                        '=' => -2,
                        '-' => -1,
                        '0' => 0,
                        '1' => 1,
                        '2' => 2,
                        _ => throw new Exception("??")
                    };
                    i++;
                }
                return new Snafu(value);
            }

            public override string ToString()
            {
                var result = "";
                var rest = value;
                var i = 0;
                while (rest != 0)
                {
                    var r = rest % 5;
                    if (r > 2)
                    {
                        r = r - 5;
                    }
                    if (r < -2)
                    {
                        r = r + 5;
                    }

                    var c = r switch
                    {
                        -2 => "=",
                        -1 => "-",
                        0 => "0",
                        1 => "1",
                        2 => "2",
                        _ => throw new Exception("Whoops")
                    };

                    result = $"{c}{result}";
                    rest = (rest - r) / 5;
                    i++;
                }
                return result;
            }

            public static Snafu operator +(Snafu lhs, Snafu rhs) => new Snafu(lhs.value + rhs.value);
        }
    }
}
