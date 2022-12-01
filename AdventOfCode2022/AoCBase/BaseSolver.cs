using System.IO;
using System.Threading.Tasks;
using AoCHelper;

namespace AoCBase
{
    public abstract class BaseSolver<U> : BaseSolver<U, U> {};

    public abstract class BaseSolver<U, V> : BaseProblem
    {
        protected override string ClassPrefix { get; } = "Solver";

        protected InputReader<T> InputReader<T>(string fileSuffix = null)
        {
            return new InputReader<T>(InputPath(fileSuffix));
        }

        protected InputReader InputReader(string fileSuffix = null)
        {
            return new InputReader(InputPath(fileSuffix));
        }

        protected InputReader<T, U> InputReader<T, U>(string fileSuffix = null)
        {
            return new InputReader<T, U>(InputPath(fileSuffix));
        }

        private string InputPath(string fileSuffix = null)
        {
            if (fileSuffix == null)
            {
                return InputFilePath;
            }

            var index = CalculateIndex().ToString("D2");
            return Path.Combine(InputFileDirPath, $"{index}.{fileSuffix}.{InputFileExtension.TrimStart('.')}");
        }

        protected abstract U Solve1();

        public override ValueTask<string> Solve_1()
        {
            return new(Solve1().ToString());
        }

        protected abstract V Solve2();

        public override ValueTask<string> Solve_2()
        {
            return new(Solve2().ToString());
        }
    }
}
