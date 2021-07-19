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
        public override GameMap GenerateMap(MapConfiguration config)
        {
            GameMap map = new GameMap(2, 2);
            MapSpaceCollection newSpaces = new MapSpaceCollection(2, 2);
            newSpaces[0, 0] = new MapSpace(0, 0, Terrain.Grassland);
            newSpaces[0, 1] = new MapSpace(0, 1, Terrain.Mountains);
            newSpaces[1, 0] = new MapSpace(1, 0, Terrain.Plains);
            newSpaces[1, 1] = new MapSpace(1, 1, null);
            map.Spaces = newSpaces;

            map.Spaces[0, 0].TerrainResources.Add(TerrainResource.Wilderness);
            map.Spaces[0, 0].Pops.Add(new Pop() { Money = 100, Job = Job.Gatherer_Wilderness });
            map.Spaces[0, 0].Pops[0].OwnedResources.Add(Resource.Wheat, 15);
            map.Spaces[0, 0].Pops[0].OwnedResources.Add(Resource.Wood, 8);
            return map;
        }
        #endregion
    }
}
