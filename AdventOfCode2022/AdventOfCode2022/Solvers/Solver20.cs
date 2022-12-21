using AoCBase;

namespace AdventOfCode2022.Solvers
{
    internal class Solver20 : BaseSolver<long>
    {
        private readonly List<int> encryptedFile;

        public Solver20()
        {
            encryptedFile = InputReader<int>().ReadInputAsLines().ToList();
        }

        protected override long Solve1()
        {
            var mixed = encryptedFile.Select((v, i) => new Item(encryptedFile, i)).ToDictionary(i => i.id);
            var i = 0;
            while (i < encryptedFile.Count)
            {
                var toMove = mixed[i];
                var steps = toMove.steps;
                if (steps != 0)
                {
                    var previous = mixed[toMove.previous];
                    var next = mixed[toMove.next];
                    previous.next = toMove.next;
                    next.previous = toMove.previous;

                    if (steps > 0)
                    {
                        var newPrevious = next;
                        var s = 1;
                        while (s < steps)
                        {
                            newPrevious = mixed[newPrevious.next];
                            s++;
                        }

                        toMove.previous = newPrevious.id;
                        toMove.next = newPrevious.next;
                        newPrevious.next = toMove.id;
                        mixed[toMove.next].previous = toMove.id;
                    }
                    else
                    {
                        // steps < 0

                        var newNext = previous;
                        var b = -1;
                        while (b > steps)
                        {
                            newNext = mixed[newNext.previous];
                            b--;
                        }

                        toMove.next = newNext.id;
                        toMove.previous = newNext.previous;
                        newNext.previous = toMove.id;
                        mixed[toMove.previous].next = toMove.id;
                    }
                }
                i++;
            }

            var zero = mixed.First(m => m.Value.value == 0).Value;
            long groveCoordinateSum = 0;
            var j = 0;
            var current = zero;
            while (j < 3000)
            {
                j++;
                current = mixed[current.next];
                if (j % 1000 == 0)
                {
                    groveCoordinateSum += current.value;
                }
            }
            return groveCoordinateSum;
        }

        protected override long Solve2()
        {
            var mixed = encryptedFile.Select((v, i) => new Item(encryptedFile, i, 811589153)).ToDictionary(i => i.id);
            var m = 0;
            while (m < 10)
            {
                var i = 0;
                while (i < encryptedFile.Count)
                {
                    var toMove = mixed[i];
                    var steps = toMove.steps;
                    if (steps != 0)
                    {
                        var previous = mixed[toMove.previous];
                        var next = mixed[toMove.next];
                        previous.next = toMove.next;
                        next.previous = toMove.previous;

                        if (steps > 0)
                        {
                            var newPrevious = next;
                            var s = 1;
                            while (s < steps)
                            {
                                newPrevious = mixed[newPrevious.next];
                                s++;
                            }

                            toMove.previous = newPrevious.id;
                            toMove.next = newPrevious.next;
                            newPrevious.next = toMove.id;
                            mixed[toMove.next].previous = toMove.id;
                        }
                        else
                        {
                            // steps < 0

                            var newNext = previous;
                            var b = -1;
                            while (b > steps)
                            {
                                newNext = mixed[newNext.previous];
                                b--;
                            }

                            toMove.next = newNext.id;
                            toMove.previous = newNext.previous;
                            newNext.previous = toMove.id;
                            mixed[toMove.previous].next = toMove.id;
                        }
                    }
                    i++;
                }
                m++;
            }

            var zero = mixed.First(m => m.Value.value == 0).Value;
            long groveCoordinateSum = 0;
            var j = 0;
            var current = zero;
            while (j < 3000)
            {
                j++;
                current = mixed[current.next];
                if (j % 1000 == 0)
                {
                    groveCoordinateSum += current.value;
                }
            }
            return groveCoordinateSum;
        }

        private class Item
        {
            public int id;
            public int previous;
            public int next;
            public long value;
            public int steps;

            public Item(List<int> encryptedFile, int initialIndex, long decryptionKey = 1)
            {
                this.id = initialIndex;
                this.previous = ModularArithmetic.NonNegativeMod(initialIndex - 1, encryptedFile.Count);
                this.next = (initialIndex + 1) % encryptedFile.Count;
                this.value = encryptedFile[initialIndex] * decryptionKey;
                this.steps = ModularArithmetic.NonNegativeMod(this.value, encryptedFile.Count - 1);
            }
        }
    }
}
