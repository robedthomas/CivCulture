using CivCulture_Model.Models.MetaComponents.UserMutables;
using GenericUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.MetaComponents.MapGenerations
{
    public class RandomMapGeneration : MapGeneration
    {
        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public override GameMap GenerateMap(MapConfiguration config, NamesDatabase namesDb, TemplatesDatabase templatesDb, IEnumerable<TechnologyTemplate> allTechs, Random seed, out List<Culture> allCultures, out List<Pop> allPops, out List<Job> allJobs)
        {
            GameMap map = new GameMap(config.Width, config.Height);
            GenerateSpaces(map, templatesDb, config, seed);
            allCultures = GenerateInitialCultures(map, config, namesDb, allTechs, seed);
            Dictionary<MapSpace, Culture> cultureLoci = AssignCultureLoci(map, allCultures, seed);
            allPops = GenerateInitialPops(map, templatesDb, config, cultureLoci, seed);
            allJobs = GenerateInitialJobs(map, templatesDb, config, seed);
            return map;
        }

        private void GenerateSpaces(GameMap map, TemplatesDatabase templatesDb, MapConfiguration config, Random seed)
        {
            HashSet<Terrain> allTerrains = new HashSet<Terrain>() { Terrain.Grassland, Terrain.Mountains, Terrain.Plains };
            for (int row = 0; row < map.Height; row++)
            {
                for (int col = 0; col < map.Width; col++)
                {
                    Terrain nextTerrain = allTerrains.PickRandom(seed);
                    map.Spaces[row, col] = new MapSpace(row, col, nextTerrain);
                    List<BuildingSlot> nextBuildingSlots = GenerateBuildingSlotsForSpace(map.Spaces[row, col], nextTerrain, templatesDb, config, seed);
                    foreach (BuildingSlot slot in nextBuildingSlots)
                    {
                        map.Spaces[row, col].BuildingSlots.Add(slot);
                    }
                }
            }
        }

        private List<BuildingSlot> GenerateBuildingSlotsForSpace(MapSpace space, Terrain terrain, TemplatesDatabase templatesDb, MapConfiguration config, Random seed)
        {
            List<BuildingSlot> output = new List<BuildingSlot>();

            Dictionary<BuildingSlotTemplate, decimal> possibleBuildingSlots = templatesDb.GetPossibleBuildingSlotsForTerrain(terrain);
            if (possibleBuildingSlots != null)
            {
                int buildingSlotCount = GetBuildingSlotCountForTerrain(terrain);
                for (int i = 0; i < buildingSlotCount; i++)
                {
                    BuildingSlotTemplate underlyingTemplate = null;
                    BuildingSlotTemplate targetTemplate = possibleBuildingSlots.PickRandomWithWeight(seed);
                    if (targetTemplate.UnderlyingSlotType.ContainsKey(terrain))
                    {
                        underlyingTemplate = targetTemplate.UnderlyingSlotType[terrain];
                    }
                    BuildingSlot newSlot = new BuildingSlot(targetTemplate, space, underlyingTemplate);
                    output.Add(newSlot);
                }
            }
            return output;
        }

        private List<Culture> GenerateInitialCultures(GameMap map, MapConfiguration config, NamesDatabase namesDb, IEnumerable<TechnologyTemplate> allTechs, Random seed)
        {
            int numCultures = seed.Next(config.MinimumNumberCultures, config.MaximumNumberCultures + 1);
            List<Culture> output = new List<Culture>(numCultures);
            for (int i = 0; i < numCultures; i++)
            {
                CultureData newCultureData = namesDb.UnusedCultures.PickRandom(seed, removeChoice: true);
                output.Add(new Culture(newCultureData.Name, null, allTechs));
            }
            return output;
        }

        private List<Pop> GenerateInitialPops(GameMap map, TemplatesDatabase templates, MapConfiguration config, Dictionary<MapSpace, Culture> cultureLoci, Random seed)
        {
            List<Pop> output = new List<Pop>();
            List<Tuple<int, int>> possibleNumPops;
            // Add minimum number of pops first
            for (int i = 0; i < config.MinimumNumberPops; i++)
            {
                MapSpace targetSpace = map.Spaces.PickRandom(seed);
                Culture closestCulture = cultureLoci[map.Spaces.GetClosestSpace(targetSpace, cultureLoci.Keys, false)];
                Pop newPop = new Pop(templates.GetPopTemplate("Hunter Gatherer"), closestCulture);
                // Add new pop to random space
                newPop.Space = targetSpace;
                output.Add(newPop);
            }
            // Add pops for each space
            foreach (MapSpace space in map.Spaces)
            {
                if (space.Terrain == Terrain.Grassland)
                {
                    possibleNumPops = new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(0, 12), // 12/20 chance of no pops
                        new Tuple<int, int>(1, 5),  // 5/20 chance of one pop
                        new Tuple<int, int>(2, 2),  // 2/20 chance of two pops
                        new Tuple<int, int>(3, 1),  // 1/20 chance of three pops
                    };
                }
                else if (space.Terrain == Terrain.Plains)
                {
                    possibleNumPops = new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(0, 14), // 14/20 chance of no pops
                        new Tuple<int, int>(1, 5),  // 6/20 chance of one pop
                        new Tuple<int, int>(2, 1),  // 1/20 chance of two pops
                    };
                }
                else if (space.Terrain == Terrain.Mountains)
                {
                    possibleNumPops = new List<Tuple<int, int>>()
                    {
                    };
                }
                else
                {
                    throw new ArgumentException($"Got unsupported Terrain \"{space.Terrain.Name}\"");
                }
                if (possibleNumPops.Count != 0)
                {
                    int numPops = possibleNumPops.PickRandomWithWeight(seed);
                    for (int i = 0; i < numPops; i++)
                    {
                        Culture closestCulture = cultureLoci[map.Spaces.GetClosestSpace(space, cultureLoci.Keys, false)];
                        Pop newPop = new Pop(templates.GetPopTemplate("Hunter Gatherer"), closestCulture);
                        newPop.Space = space;
                        output.Add(newPop);
                    }
                }
            }
            return output;
        }

        private List<Job> GenerateInitialJobs(GameMap map, TemplatesDatabase templates, MapConfiguration config, Random seed)
        {
            List<Job> output = new List<Job>();
            foreach (MapSpace space in map.Spaces)
            {
                foreach (BuildingSlot slot in space.BuildingSlots)
                {
                    foreach (JobTemplate template in slot.Template.ChildJobTemplates)
                    {
                        Job newJob = new Job(template, slot);
                        newJob.Space = space;
                        output.Add(newJob);
                    }
                }
            }
            return output;
        }

        private static Dictionary<MapSpace, Culture> AssignCultureLoci(GameMap map, List<Culture> cultures, Random seed)
        {
            Dictionary<MapSpace, Culture> cultureLoci = new Dictionary<MapSpace, Culture>();
            HashSet<MapSpace> spaces = new HashSet<MapSpace>(map.Spaces);
            foreach (Culture c in cultures)
            {
                cultureLoci.Add(spaces.PickRandom(seed, removeChoice: true), c);
            }
            return cultureLoci;
        }

        private int GetBuildingSlotCountForTerrain(Terrain targetTerrain)
        {
            if (targetTerrain == Terrain.Grassland)
            {
                return 4;
            }
            else if (targetTerrain == Terrain.Plains)
            {
                return 4;
            }
            else if (targetTerrain == Terrain.Mountains)
            {
                return 0;
            }
            else
            {
                throw new ArgumentException($"Encountered unsupported terrain type while determing number of building slots: {targetTerrain.Name}");
            }
        }
        #endregion
    }
}
