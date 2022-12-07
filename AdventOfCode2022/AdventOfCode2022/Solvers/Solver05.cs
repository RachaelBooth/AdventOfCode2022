using AoCBase;


namespace AdventOfCode2022.Solvers
{
    public class Solver05 : BaseSolver<string>
    {
        private readonly Dictionary<int, List<char>> initialCrates;
        private readonly List<Move> moves;

        public Solver05()
        {
            var input = InputReader<string, Move>().ReadAsMultipartLines();
            moves = input.Item2;
            initialCrates = new Dictionary<int, List<char>>();
            for (var row = 0; row < input.Item1.Count; row++)
            {
                for (var col = 1; col <= 9; col++)
                {
                    var c = input.Item1[row][(col * 4) - 3];
                    if (char.IsUpper(c))
                    {
                        initialCrates.AddOptions(col, c);
                    }
                }
            }            
        }

        protected override string Solve1()
        {
           var crates = new Dictionary<int, List<char>> (initialCrates.ToList());
            foreach (var move in moves)
            {
                var removed = crates.RemoveFirst(move.From, move.Crates);
                removed.Reverse();
                crates.AddOptionsToStart(move.To, removed.ToArray());
            }
            var tops = "";
            for (var col = 1; col <= 9; col++)
            {
                tops = tops + crates.GetValues(col)[0];
            }
            return tops;
        }

        protected override string Solve2()
        {
            var crates = new Dictionary<int, List<char>>(initialCrates.ToList());
            foreach (var move in moves)
            {
                var removed = crates.RemoveFirst(move.From, move.Crates);
                crates.AddOptionsToStart(move.To, removed.ToArray());
            }
            var tops = "";
            for (var col = 1; col <= 9; col++)
            {
                tops = tops + crates.GetValues(col)[0];
            }
            return tops;
        }

        private class Move
        {
            public int Crates;
            public int From;
            public int To;

            public Move(string crates, string from, string to)
            {
                Crates = int.Parse(crates);
                From = int.Parse(from);
                To = int.Parse(to);
            }

            public static Move Parse(string line)
            {
                var parts = line.Split(' ');
                return new Move(parts[1], parts[3], parts[5]);
            }
        }
    }
}
