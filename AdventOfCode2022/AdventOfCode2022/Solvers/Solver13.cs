using AoCBase;

namespace AdventOfCode2022.Solvers
{
    internal class Solver13 : BaseSolver<int>
    {
        protected override int Solve1()
        {
            var packetPairs = InputReader<PacketData>().ReadInputAsGroupedLines().ToList();
            var correctOrderSum = 0;
            var i = 0;
            while (i < packetPairs.Count)
            {
                var comparison = Compare(packetPairs[i][0], packetPairs[i][1]);
                if (comparison < 0)
                {
                    correctOrderSum += i + 1;
                }
                i++;
            }
            return correctOrderSum;
        }

        protected override int Solve2()
        {
            var packets = InputReader<PacketData>().ReadInputAsLinesSkippingBlanks().ToList();
            var firstDivider = PacketData.Parse("[[2]]");
            var secondDivider = PacketData.Parse("[[6]]");
            packets.Add(firstDivider);
            packets.Add(secondDivider);
            packets.Sort((first, second) => Compare(first, second));
            return (packets.IndexOf(firstDivider) + 1) * (packets.IndexOf(secondDivider) + 1);
        }

        // Negative means left is before right (= correct order)
        private int Compare(PacketData left, PacketData right)
        {
            if (left.IsInt() && right.IsInt())
            {
                return left.Value() - right.Value();
            }

            var adjustedLeft = left;
            var adjustedRight = right;

            if (left.IsInt())
            {
                adjustedLeft = new PacketData(new List<PacketData>() { left });
            }

            if (right.IsInt())
            {
                adjustedRight = new PacketData(new List<PacketData>() { right });
            }

            var leftValues = adjustedLeft.Values();
            var rightValues = adjustedRight.Values();

            var i = 0;
            while (i < leftValues.Count)
            {
                if (i >= rightValues.Count)
                {
                    // Wrong order
                    return 1;
                }

                var result = Compare(leftValues[i], rightValues[i]);
                if (result != 0)
                {
                    return result;
                }

                i++;
            }

            if (i < rightValues.Count)
            {
                // more right values than left ones
                return -1;
            }

            return 0;
        }

        private class PacketData
        {
            private readonly int? _value;
            private readonly List<PacketData>? _values;

            public PacketData(List<PacketData> values)
            {
                _values = values;
            }

            public PacketData(int value)
            {
                _value = value;
            }

            public static PacketData Parse(string line)
            {
                if (line.StartsWith('['))
                {
                    var parts = new List<string>();
                    // Ignore the first and last character - the outer []
                    var i = 1;
                    var part = "";
                    var level = 0;
                    while (i < line.Length - 1)
                    {
                        if (line[i] == ',' && level == 0)
                        {
                            parts.Add(part);
                            part = "";
                        } 
                        else if (line[i] == '[')
                        {
                            level++;
                            part += line[i];
                        }
                        else if (line[i] == ']')
                        {
                            level--;
                            part += line[i];
                        }
                        else
                        {
                            part += line[i];
                        }

                        i++;
                    }

                    if (part != "")
                    {
                        parts.Add(part);
                    }

                    return new PacketData(parts.Select(part => Parse(part)).ToList());
                }

                return new PacketData(int.Parse(line));
            }

            public bool IsInt()
            {
                return _value.HasValue;
            }

            public int Value()
            {
                if (!IsInt())
                {
                    throw new Exception("No");
                }

                return _value.Value;
            }

            public List<PacketData> Values()
            {
                if (IsInt())
                {
                    throw new Exception("Also no");
                }

                return _values;
            }
        }
    }
}
