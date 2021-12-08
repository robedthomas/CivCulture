using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.MetaComponents.MapGenerations
{
    public class TestMapGeneration : MapGeneration
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public override GameMap GenerateMap(MapConfiguration config, Random seed, out List<Pop> allPops, out List<Job> allJobs)
        {
            if (!TerrainResource.Initialized)
            {
                TerrainResource.InitializeTerrainResources();
            }
            allPops = new List<Pop>();
            allJobs = new List<Job>();
            GameMap map = new GameMap(2, 2);
            MapSpaceCollection newSpaces = new MapSpaceCollection(2, 2);
            newSpaces[0, 0] = new MapSpace(0, 0, Terrain.Grassland, TerrainResource.Wilderness);
            newSpaces[0, 1] = new MapSpace(0, 1, Terrain.Mountains);
            newSpaces[1, 0] = new MapSpace(1, 0, Terrain.Plains);
            newSpaces[1, 1] = new MapSpace(1, 1, null);
            map.Spaces = newSpaces;

            Job job = new Job(JobTemplate.Gatherer_Wilderness);
            allJobs.Add(job);
            Pop pop = new Pop(PopTemplate.Laborer) { Money = 100, Job = job, Space = map.Spaces[0, 0] };
            pop.OwnedResources.Add(Resource.Wheat, 15);
            pop.OwnedResources.Add(Resource.Wood, 8);
            allPops.Add(pop);

            return map;
        }
        #endregion
    }
}
