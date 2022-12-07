using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver07 : BaseSolver<long>
    {
        private class Directory
        {
            private ValueCollection<(string, long)> files;
            private Dictionary<string, Directory> subdirectories;
            public string name;
            private Directory? parent;
            public long totalSize;
            public long directSize;

            public Directory(string name, Directory? parent = null)
            {
                this.name = name;
                files = new ValueCollection<(string, long)>();
                subdirectories = new Dictionary<string, Directory>();
                this.parent = parent;
                totalSize = 0;
                directSize = 0;
            }

            public void AddFile(string name, long size)
            {
                files.Add((name, size));
                directSize += size;
                AddSubdirectoryFile(size);
            }

            public void AddSubdirectoryFile(long size)
            {
                totalSize += size;
                if (parent != null)
                {
                    parent.AddSubdirectoryFile(size);
                }
            }

            public bool SubdirectoryExists(string name)
            {
                return subdirectories.ContainsKey(name);
            }

            // Adds if not present
            public Directory GetSubdirectory(string name)
            {
                return subdirectories.ReadWithDefault(name, () => new Directory(name, this), true);
            }

            public Directory Parent()
            {
                if (parent == null)
                {
                    throw new Exception("Oh dear");
                }

                return parent;
            }
        }

        private Directory homeDir;
        private List<Directory> allDirectories;


        public Solver07()
        {
            allDirectories = new List<Directory>();
            var lines = InputReader<string>().ReadInputAsLines();
            homeDir = new Directory("/");
            allDirectories.Add(homeDir);
            var currentDirectory = homeDir;
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                if (parts[0] == "$")
                {
                    // Command
                    var command = parts[1];
                    if (command == "cd")
                    {
                        if (parts[2] == "/")
                        {
                            currentDirectory = homeDir;
                        } 
                        else if (parts[2] == "..")
                        {
                            currentDirectory = currentDirectory.Parent();
                        } 
                        else
                        {
                            var isNew = !currentDirectory.SubdirectoryExists(parts[2]);
                            currentDirectory = currentDirectory.GetSubdirectory(parts[2]);
                            if (isNew)
                            {
                                allDirectories.Add(currentDirectory);
                            }
                        }
                    }

                    // Otherwise in listing until another command
                }
                else
                {
                    // Must be listing current directory contents
                    if (parts[0] == "dir")
                    {
                        if (!currentDirectory.SubdirectoryExists(parts[1]))
                        {
                            // adds if not present
                            var added = currentDirectory.GetSubdirectory(parts[1]);
                            allDirectories.Add(added);
                        }
                    }
                    else
                    {
                        // Is file
                        var size = long.Parse(parts[0]);
                        var name = parts[1];
                        currentDirectory.AddFile(name, size);
                    }
                }
            }
        }

        protected override long Solve1()
        {
            return allDirectories.Where(dir => dir.totalSize <= 100000).Sum(dir => dir.totalSize);
        }

        protected override long Solve2()
        {
            var spaceRequired = 30000000 - (70000000 - homeDir.totalSize);
            return allDirectories.Select(dir => dir.totalSize).Where(size => size >= spaceRequired).Min();
        }
    }
}
