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
        public override GameMap GenerateMap(MapConfiguration config, NamesDatabase namesDb, Random seed, out List<Culture> allCultures, out List<Pop> allPops, out List<Job> allJobs)
        {
            GameMap map = new GameMap(config.Width, config.Height);
            GenerateSpaces(map, config, seed);
            allCultures = GenerateInitialCultures(map, config, namesDb, seed);
            Dictionary<MapSpace, Culture> cultureLoci = AssignCultureLoci(map, allCultures, seed);
            allPops = GenerateInitialPops(map, config, cultureLoci, seed);
            allJobs = GenerateInitialJobs(map, config, seed);
            return map;
        }

        private void GenerateSpaces(GameMap map, MapConfiguration config, Random seed)
        {
            HashSet<Terrain> allTerrains = new HashSet<Terrain>() { Terrain.Grassland, Terrain.Mountains, Terrain.Plains };
            for (int row = 0; row < map.Height; row++)
            {
                for (int col = 0; col < map.Width; col++)
                {
                    Terrain nextTerrain = allTerrains.PickRandom(seed);
                    List<TerrainResource> nextTerrainResources = GenerateTerrainResources(nextTerrain, config, seed);
                    map.Spaces[row, col] = new MapSpace(row, col, nextTerrain, nextTerrainResources.ToArray());
                }
            }
        }

        private List<TerrainResource> GenerateTerrainResources(Terrain terrain, MapConfiguration config, Random seed)
        {
            List<TerrainResource> output = new List<TerrainResource>();
            List<Tuple<int, int>> possibleNumResources;
            List<Tuple<TerrainResourceTemplate, int>> possibleResources;
            if (terrain == Terrain.Grassland)
            {
                possibleNumResources = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(1, 1), // 1/5 chance of one resource
                    new Tuple<int, int>(2, 3), // 3/5 chance of two resources
                    new Tuple<int, int>(3, 1), // 1/5 chance of three resources
                };
                possibleResources = new List<Tuple<TerrainResourceTemplate, int>>()
                {
                    new Tuple<TerrainResourceTemplate, int>(TerrainResourceTemplate.Wilderness, 1), // 1/2 chance of wilderness
                    new Tuple<TerrainResourceTemplate, int>(TerrainResourceTemplate.Wheat, 1),      // 1/2 chance of wheat
                };
            }
            else if (terrain == Terrain.Plains)
            {
                possibleNumResources = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(1, 2), // 2/6 chance of one resource
                    new Tuple<int, int>(2, 3), // 3/6 chance of two resources
                    new Tuple<int, int>(3, 1), // 1/6 chance of three resources
                };
                possibleResources = new List<Tuple<TerrainResourceTemplate, int>>()
                {
                    new Tuple<TerrainResourceTemplate, int>(TerrainResourceTemplate.Wilderness, 1), // 1/2 chance of wilderness
                    new Tuple<TerrainResourceTemplate, int>(TerrainResourceTemplate.Wheat, 1),      // 1/2 chance of wheat
                };
            }
            else if (terrain == Terrain.Mountains)
            {
                possibleNumResources = new List<Tuple<int, int>>()
                {
                };
                possibleResources = new List<Tuple<TerrainResourceTemplate, int>>()
                {
                };
            }
            else
            {
                throw new ArgumentException($"Got unsupported Terrain \"{terrain.Name}\"");
            }
            if (possibleNumResources.Count != 0 && possibleResources.Count != 0)
            {
                int numResources = possibleNumResources.PickRandomWithWeight(seed);
                for (int i = 0; i < numResources; i++)
                {
                    output.Add(new TerrainResource(possibleResources.PickRandomWithWeight(seed)));
                }
            }
            return output;
        }

        private List<Culture> GenerateInitialCultures(GameMap map, MapConfiguration config, NamesDatabase namesDb, Random seed)
        {
            int numCultures = seed.Next(config.MinimumNumberCultures, config.MaximumNumberCultures + 1);
            List<Culture> output = new List<Culture>(numCultures);
            for (int i = 0; i < numCultures; i++)
            {
                CultureData newCultureData = namesDb.UnusedCultures.PickRandom(seed, removeChoice: true);
                output.Add(new Culture(newCultureData.Name, null));
            }
            return output;
        }

        private List<Pop> GenerateInitialPops(GameMap map, MapConfiguration config, Dictionary<MapSpace, Culture> cultureLoci, Random seed)
        {
            List<Pop> output = new List<Pop>();
            List<Tuple<int, int>> possibleNumPops;
            // Add minimum number of pops first
            for (int i = 0; i < config.MinimumNumberPops; i++)
            {
                MapSpace targetSpace = map.Spaces.PickRandom(seed);
                Culture closestCulture = cultureLoci[map.Spaces.GetClosestSpace(targetSpace, cultureLoci.Keys, false)];
                Pop newPop = new Pop(PopTemplate.HunterGatherer, closestCulture);
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
                        Pop newPop = new Pop(PopTemplate.HunterGatherer, closestCulture);
                        newPop.Space = space;
                        output.Add(newPop);
                    }
                }
            }
            return output;
        }

        private List<Job> GenerateInitialJobs(GameMap map, MapConfiguration config, Random seed)
        {
            List<Job> output = new List<Job>();
            foreach (MapSpace space in map.Spaces)
            {
                foreach (TerrainResource resource in space.TerrainResources)
                {
                    foreach (JobTemplate template in resource.Template.ChildJobTemplates)
                    {
                        Job newJob = new Job(template, resource);
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
        #endregion
    }
}
