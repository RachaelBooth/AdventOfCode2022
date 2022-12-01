# README

Advent of code is an excellent set of programming puzzles released each year, which can be found at [adventofcode.com](https://adventofcode.com).

This contains my 2022 solutions and the current slowly-building-out personal helpers I've been setting up, which still haven't gone anywhere near being their own package.

This uses the [AoCHelper](https://github.com/eduherminio/AoCHelper) package for running, while my own helpers (in AoCBase) add some input parsing and other utilities.

### Usage

This is really to remind me, to be honest.

For a new day;
1. Add a `SolverXX` class which inherits `BaseSolver`, probably in its own folder so you can split out into several files as needed
2. Copy and paste the input to `Inputs\XX.txt`, and ensure the file is set to be copied to the output directory
3. Solve the problem!

Running the solution will run the most recent day. Edit `Program.cs` to solve all or solve a specific day.