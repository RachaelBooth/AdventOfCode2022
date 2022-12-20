using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Solvers
{
    internal class Solver19 : BaseSolver<int>
    {
        private List<Blueprint> blueprints;

        public Solver19()
        {
            blueprints = InputReader<Blueprint>().ReadInputAsLines().ToList();
        }

        protected override int Solve1()
        {
            return blueprints.Sum(blueprint => QualityLevel(blueprint));
        }

        protected override int Solve2()
        {
            var first = FindLargestGeodesOpenedForBlueprint(blueprints[0], 32);
            var second = FindLargestGeodesOpenedForBlueprint(blueprints[1], 32);
            var third = FindLargestGeodesOpenedForBlueprint(blueprints[2], 32);
            return first * second * third;
        }

        private static int QualityLevel(Blueprint blueprint)
        {
            return blueprint.GetId() * FindLargestGeodesOpenedForBlueprint(blueprint, 24);
        }

        private static int FindLargestGeodesOpenedForBlueprint(Blueprint blueprint, int minutes)
        {
            var otherMaterials = new List<Material> { Material.Ore, Material.Clay, Material.Obsidian, Material.Geode };
            var maxCost = blueprint.GetCostForRobotType(Material.Ore);
            // N.B. Ore and clay both only cost ore
            maxCost = (Math.Max(blueprint.GetCostForRobotType(Material.Clay).ore, maxCost.ore), 0, 0, 0);
            // N.B. Obsidian always costs ore and clay
            maxCost = (Math.Max(blueprint.GetCostForRobotType(Material.Obsidian).ore, maxCost.ore), blueprint.GetCostForRobotType(Material.Obsidian).clay, 0, 0);
            // Geode always costs ore and obsidian
            maxCost = (Math.Max(blueprint.GetCostForRobotType(Material.Geode).ore, maxCost.ore), maxCost.clay, blueprint.GetCostForRobotType(Material.Geode).obsidian, 0);

            var states = new HashSet<((int ore, int clay, int obsidian, int geode) inventory, (int ore, int clay, int obsidian, int geode) production)>();
            states.Add(((0, 0, 0, 0), (1, 0, 0, 0)));

            var time = 0;
            while (time < minutes)
            {
                time++;
                var newStates = new HashSet<((int ore, int clay, int obsidian, int geode) inventory, (int ore, int clay, int obsidian, int geode) production)>();
                foreach (var state in states)
                {
                    var nextPotentialStates = new HashSet<((int ore, int clay, int obsidian, int geode) inventory, (int ore, int clay, int obsidian, int geode) production)>();

                    // Try saving up or building remaining robots
                    nextPotentialStates.Add((Sum(state.inventory, state.production), state.production));
                    foreach (var material in otherMaterials)
                    {
                        var afterCost = SubtractCost(state.inventory, material, blueprint);
                        if (IsGreaterThanOrEqualTo(afterCost, (0, 0, 0, 0)))
                        {
                            nextPotentialStates.Add((Sum(afterCost, state.production), AddProduction(state.production, material)));
                        }
                    }

                    foreach (var potential in nextPotentialStates)
                    {
                        if (WouldGivePointlessProduction(potential.production, maxCost) || newStates.Any(s => IsBetterThan(s, potential, minutes - time))) 
                        {
                            // Don't bother, it's either:
                            // 1. Got more production than we can use in a turn
                            // 2. Strictly worse than something we've already got
                        }
                        else
                        {
                            newStates.RemoveWhere(s => IsBetterThan(potential, s, minutes - time));
                            newStates.Add(potential);
                        }
                    }
                }
                states = newStates;
            }

            return states.Max(s => s.inventory.geode);
        }

        // True if first is better than second
        public static bool IsBetterThan(((int ore, int clay, int obsidian, int geode) inventory, (int ore, int clay, int obsidian, int geode) production) first, ((int ore, int clay, int obsidian, int geode) inventory, (int ore, int clay, int obsidian, int geode) production) second, int remainingTime)
        {
            // Assume that for all of the remaining time the second could add a geode robot every second and the first can't add any
            var secondMaxGeodes = second.inventory.geode + remainingTime * second.production.geode + (remainingTime * (remainingTime + 1)) / 2;
            var firstMinGeodes = first.inventory.geode + remainingTime * first.production.geode;

            if (firstMinGeodes >= secondMaxGeodes)
            {
                return true;
            }

            if (IsGreaterThanOrEqualTo(first.inventory, second.inventory) && IsGreaterThanOrEqualTo(first.production, second.production))
            {
                return true;
            }

            // Could we add some more clever checks?
            return false;
        }

        public static bool IsGreaterThanOrEqualTo((int ore, int clay, int obsidian, int geode) first, (int ore, int clay, int obsidian, int geode) second)
        {
            return first.ore >= second.ore && first.clay >= second.clay && first.obsidian >= second.obsidian && first.geode >= second.geode;
        }

        public static bool WouldGivePointlessProduction((int ore, int clay, int obsidian, int geode) production, (int ore, int clay, int obsidian, int geode) maxCosts)
        {
            return production.ore > maxCosts.ore || production.clay > maxCosts.clay || production.obsidian > maxCosts.obsidian;
        }

        private static (int ore, int clay, int obsidian, int geode) SubtractCost((int ore, int clay, int obsidian, int geode) inventory, Material robotType, Blueprint blueprint)
        {
            var cost = blueprint.GetCostForRobotType(robotType);
            return (inventory.ore - cost.ore, inventory.clay - cost.clay, inventory.obsidian - cost.obsidian, inventory.geode - cost.geode);
        }

        private static (int ore, int clay, int obsidian, int geode) Sum((int ore, int clay, int obsidian, int geode) first, (int ore, int clay, int obsidian, int geode) second)
        {
            return (first.ore + second.ore, first.clay + second.clay, first.obsidian + second.obsidian, first.geode + second.geode);
        }

        private static (int ore, int clay, int obsidian, int geode) AddProduction((int ore, int clay, int obsidian, int geode) currentProduction, Material newRobot)
        {
            return (currentProduction.ore + (newRobot == Material.Ore ? 1 : 0), currentProduction.clay + (newRobot == Material.Clay ? 1 : 0), currentProduction.obsidian + (newRobot == Material.Obsidian ? 1 : 0), currentProduction.geode + (newRobot == Material.Geode ? 1 : 0));
        }

        private enum Material
        {
            Ore,
            Clay,
            Obsidian,
            Geode
        }

        private class Blueprint
        {
            private Dictionary<Material, (int ore, int clay, int obsidian, int geode)> costs;
            private int id;

            public Blueprint(string line)
            {
                var parts = line.Split(' ');
                costs = new Dictionary<Material, (int ore, int clay, int obsidian, int geode)>();
                // Conveniently only the numbers change between blueprints in the input
                costs.Add(Material.Ore, (int.Parse(parts[6]), 0, 0, 0));
                costs.Add(Material.Clay, (int.Parse(parts[12]), 0, 0, 0));
                costs.Add(Material.Obsidian, (int.Parse(parts[18]), int.Parse(parts[21]), 0, 0));
                costs.Add(Material.Geode, (int.Parse(parts[27]), 0, int.Parse(parts[30]), 0));
                id = int.Parse(parts[1].TrimEnd(':'));
            }

            public (int ore, int clay, int obsidian, int geode) GetCostForRobotType(Material robotType)
            {
                return costs[robotType];
            }

            public int GetId()
            {
                return id;
            }

            public static Blueprint Parse(string line)
            {
                return new Blueprint(line);
            }
        }
    }
}
