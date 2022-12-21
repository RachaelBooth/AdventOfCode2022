using AoCBase;

namespace AdventOfCode2022.Solvers
{
    internal class Solver21 : BaseSolver<long>
    {
        private Dictionary<string, long> yells;
        private Dictionary<string, (string firstInput, string secondInput, Func<long, long, long> calculation)> calculations;
        private Dictionary<string, (string firstInput, string secondInput, Func<Polynomial, Polynomial, Polynomial> calculation)> polynomialsInHumn;

        const string humn = "humn";

        public Solver21()
        {
            yells = new Dictionary<string, long>();
            calculations = new Dictionary<string, (string firstInput, string secondInput, Func<long, long, long> calculation)>();
            polynomialsInHumn = new Dictionary<string, (string firstInput, string secondInput, Func<Polynomial, Polynomial, Polynomial> calculation)>();

            foreach (var monkey in InputReader<string>().ReadInputAsLines())
            {
                var parts = monkey.Split(" ");
                if (parts.Length == 2)
                {
                    yells.Add(parts[0].TrimEnd(':'), long.Parse(parts[1]));
                }
                else
                {
                    calculations.Add(parts[0].TrimEnd(':'), (parts[1], parts[3], (x, y) => parts[2] switch
                    {
                        "*" => x * y,
                        "+" => x + y,
                        "/" => x / y,
                        "-" => x - y,
                        _ => throw new Exception("Unexpected operator")
                    }));

                    polynomialsInHumn.Add(parts[0].TrimEnd(':'), (parts[1], parts[3], (Polynomial x, Polynomial y) => parts[2] switch
                    {
                        "*" => x * y,
                        "+" => x + y,
                        "/" => x / y,
                        "-" => x - y,
                        _ => throw new Exception("Unexpected operator")
                    }));
                }
            }
        }

        protected override long Solve1()
        {
            return GetYell("root");
        }

        private long GetYell(string monkey, long? overrideHumn = null)
        {
            if (overrideHumn.HasValue && monkey == "humn")
            {
                return overrideHumn.Value;
            }

            if (yells.ContainsKey(monkey))
            {
                return yells[monkey];
            }

            var calc = calculations[monkey];
            // We assume there's not going to be any bad loops...
            return calc.calculation(GetYell(calc.firstInput, overrideHumn), GetYell(calc.secondInput, overrideHumn));
        }

        protected override long Solve2()
        {
            var first = GetPolynomial(calculations["root"].firstInput);
            var second = GetPolynomial(calculations["root"].secondInput);

            var toSolve = first - second;
            // Will throw if not linear, but we live in hope
            var solution = toSolve.Solve();

            // solution is probably not an integer because precision
            // check that the rounded answer actually works (probably not strictly necessary but...)
            var rounded = (long) Math.Round(solution);
            if (GetYell(calculations["root"].firstInput, rounded) == GetYell(calculations["root"].secondInput, rounded))
            {
                return rounded;
            }

            throw new Exception("???");
        }

        private Polynomial GetPolynomial(string monkey)
        {
            if (monkey == humn)
            {
                return new Polynomial(0, 1);
            }

            if (yells.ContainsKey(monkey))
            {
                return new Polynomial(yells[monkey]);
            }

            var poly = polynomialsInHumn[monkey];
            var firstInput = GetPolynomial(poly.firstInput);
            var secondInput = GetPolynomial(poly.secondInput);

            return poly.calculation(firstInput, secondInput);
        }
    }
}
