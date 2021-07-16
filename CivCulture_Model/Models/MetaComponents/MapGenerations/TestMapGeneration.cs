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
            map.Spaces[0, 0] = new MapSpace(0, 0, Terrain.Grassland);
            map.Spaces[0, 1] = new MapSpace(0, 1, Terrain.Mountains);
            map.Spaces[1, 0] = new MapSpace(1, 0, Terrain.Plains);
            map.Spaces[1, 1] = new MapSpace(1, 1, null);

            map.Spaces[0, 0].TerrainResources.Add(TerrainResource.Wilderness);
            map.Spaces[0, 0].Pops.Add(new Pop() { Money = 100, Job = Job.Gatherer_Wilderness });
            map.Spaces[0, 0].Pops[0].OwnedResources.Add(new Tuple<Resource, int>(Resource.Wheat, 15));
            map.Spaces[0, 0].Pops[0].OwnedResources.Add(new Tuple<Resource, int>(Resource.Wood, 8));
            return map;
        }
        #endregion
    }
}
