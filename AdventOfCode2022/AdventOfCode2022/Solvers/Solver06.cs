using AoCBase;

namespace AdventOfCode2022.Solvers
{
    internal class Solver06 : BaseSolver<int>
    {
        private List<char> datastream;

        public Solver06()
        {
            datastream = InputReader<char>().ReadInputAsSingleLineByChar();
        }

        protected override int Solve1()
        {
            return FindEndOfFirstMarkerOfDistinctCharacters(datastream, 4);
        }

        protected override int Solve2()
        {
            return FindEndOfFirstMarkerOfDistinctCharacters(datastream, 14);
        }

        private static int FindEndOfFirstMarkerOfDistinctCharacters(List<char> buffer, int markerLength)
        {
            var processed = markerLength;

            while (processed < buffer.Count)
            {
                var section = buffer.GetRange(processed - markerLength, markerLength).ToHashSet();
                if (section.Count == markerLength)
                {
                    return processed;
                }
                processed++;
            }
            throw new Exception("Whoops");
        }
    }
}
