using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AoCBase.Vectors;

namespace AoCBase
{
    public class InputReader : InputReader<string>
    {
        public InputReader(string filePath) : base(filePath) { }

        public Dictionary<(int x, int y), char> Parse2DimensionalGrid()
        {
            return Parse2DimensionalGrid(c => c);
        }

        public Dictionary<(int x, int y), T> Parse2DimensionalGrid<T>(Func<char, T> characterMap)
        {
            return ParseGrid(l => l, characterMap);
        }

        public Dictionary<(int x, int y, int z), char> Parse3DimensionalGrid()
        {
            return Parse3DimensionalGrid(c => c);
        }

        public Dictionary<(int x, int y, int z), T> Parse3DimensionalGrid<T>(Func<char, T> characterMap)
        {
            return ParseGrid(l => l.ToThreeDimensions(), characterMap);
        }

        public Dictionary<(int x, int y, int z, int w), T> Parse4DimensionalGrid<T>(Func<char, T> characterMap)
        {
            return ParseGrid(l => l.ToFourDimensions(), characterMap);
        }

        public Dictionary<T, U> ParseGrid<T, U>(Func<(int x, int y), T> locationMap, Func<char, U> characterMap)
        {
            var grid = new Dictionary<T, U>();
            var y = 0;
            foreach (var line in ReadInputAsLines())
            {
                var x = 0;
                while (x < line.Length)
                {
                    grid.Add(locationMap((x, y)), characterMap(line[x]));
                    x++;
                }

                y++;
            }

            return grid;
        }
    }

    // QQ - could unify this a bit with below
    public class InputReader<T, U>
    {
        private readonly string inputFilePath;

        public InputReader(string filePath)
        {
            inputFilePath = filePath;
        }

        public (List<T>, List<U>) ReadAsMultipartLines()
        {
            var reader = new StreamReader(inputFilePath);
            string line;

            var t = new List<T>();
            var u = new List<U>();
            var inFirstSection = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (line != "")
                {
                    if (inFirstSection)
                    {
                        t.Add(ParseLine<T>(line));
                    }
                    else
                    {
                        u.Add(ParseLine<U>(line));
                    }
                }
                else
                {
                    inFirstSection = false;
                }
            }
            return (t, u);
        }

        private V ParseLine<V>(string line)
        {
            if (typeof(V) == typeof(string))
            {
                // Ewww
                return (V)Convert.ChangeType(line, typeof(V));
            }
            var parse = typeof(V).GetMethod("Parse", new Type[] { typeof(string) });
            return (V)parse.Invoke(this, new[] { line });
        }
    }

    public class InputReader<T>
    {
        private readonly string inputFilePath;

        public InputReader(string filePath)
        {
            inputFilePath = filePath;
        }

        public IEnumerable<T> ReadInputAsLines()
        {
            var reader = new StreamReader(inputFilePath);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseLine(line);
            }

            reader.Close();
        }

        public IEnumerable<List<T>> ReadInputAsBatchedLines(int batchSize)
        {
            var reader = new StreamReader(inputFilePath);
            string line;

            var currentLines = new List<T>();
            while ((line = reader.ReadLine()) != null)
            {
                currentLines.Add(ParseLine(line));
                if (currentLines.Count == batchSize)
                {
                    yield return currentLines.ToList();
                    currentLines = new List<T>();
                }
            }

            // If the final group is not empty but didn't reach the batch size then return it
            if (currentLines.Any())
            {
                yield return currentLines;
            }

            reader.Close();
        }

        public IEnumerable<T> ReadInputAsLineGroups()
        {
            var reader = new StreamReader(inputFilePath);
            string line;

            var currentLines = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "")
                {
                    yield return ParseLineGroup(currentLines);
                    currentLines = new List<string>();
                }
                else
                {
                    currentLines.Add(line);
                }
            }
            // Ensures we return the final group (will get null for end of file rather than a final blank line)
            yield return ParseLineGroup(currentLines);

            reader.Close();
        }

        public IEnumerable<List<T>> ReadInputAsGroupedLines()
        {
            var reader = new StreamReader(inputFilePath);
            string line;

            var currentLines = new List<T>();
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "")
                {
                    yield return currentLines;
                    currentLines = new List<T>();
                }
                else
                {
                    currentLines.Add(ParseLine(line));
                }
            }
            // Ensures we return the final group (will get null for end of file rather than a final blank line)
            yield return currentLines;

            reader.Close();
        }

        public Dictionary<string, List<T>> ReadInputAsGroupedLinesByHeader()
        {
            var reader = new StreamReader(inputFilePath);
            string line;

            var dict = new Dictionary<string, List<T>>();

            var currentKey = "";
            var currentLines = new List<T>();
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "")
                {
                    dict.Add(currentKey, currentLines);
                    currentKey = "";
                    currentLines = new List<T>();
                }
                else
                {
                    if (currentKey == "")
                    {
                        currentKey = line;
                    }
                    else
                    {
                        currentLines.Add(ParseLine(line));
                    }
                }
            }
            dict.Add(currentKey, currentLines);

            reader.Close();
            return dict;
        }

        public List<T> ReadInputAsSingleCommaSeparatedLine()
        {
            var reader = new StreamReader(inputFilePath);
            var line = reader.ReadLine();
            var result = line.Split(',').Select(p => ParseLine(p)).ToList();
            if (reader.ReadLine() != null)
            {
                throw new Exception("Expected single line but got multiple");
            }
            reader.Close();
            return result;
        }

        public List<T> ReadInputAsSingleLineByChar()
        {
            var reader = new StreamReader(inputFilePath);
            var line = reader.ReadLine();
            var result = line.ToCharArray().Select(c => ParseChar(c)).ToList();
            if (reader.ReadLine() != null)
            {
                throw new Exception("Expected single line but got multiple");
            }
            reader.Close();
            return result;
        }

        private T ParseLine(string line)
        {
            if (typeof(T) == typeof(string))
            {
                // Ewww
                return (T)Convert.ChangeType(line, typeof(T));
            }
            var parse = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
            return (T)parse.Invoke(this, new[] { line });
        }

        private T ParseLineGroup(List<string> lines)
        {
            var parse = typeof(T).GetMethod("Parse", new Type[] { typeof(List<string>) });
            return (T)parse.Invoke(this, new[] { lines });
        }

        private T ParseChar(char c)
        {
            if (typeof(T) == typeof(char))
            {
                return (T)Convert.ChangeType(c, typeof(T));
            }
            var parse = typeof(T).GetMethod("Parse", new Type[] { typeof(char) });
            return (T)parse.Invoke(this, new object[] { c });
        }
    }
}
