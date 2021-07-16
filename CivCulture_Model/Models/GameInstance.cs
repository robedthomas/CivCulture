using CivCulture_Model.Events;
using CivCulture_Model.Models.MetaComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class GameInstance : BaseModel
    {
        #region Fields
        private GameMap map;
        private MapGeneration mapGeneration;
        private MapConfiguration mapConfig;
        #endregion

        #region Events
        public event ValueChangedEventHandler<GameMap> MapChanged;
        public event ValueChangedEventHandler<MapGeneration> MapGenerationChanged;
        public event ValueChangedEventHandler<MapConfiguration> MapConfigChanged;
        #endregion

        #region Properties
        public GameMap Map
        {
            get => map;
            set
            {
                if (map != value)
                {
                    GameMap oldValue = map;
                    map = value;
                    MapChanged?.Invoke(this, new ValueChangedEventArgs<GameMap>(oldValue, map));
                }
            }
        }

        public MapGeneration MapGeneration
        {
            get => mapGeneration;
            set
            {
                if (mapGeneration != value)
                {
                    MapGeneration oldValue = mapGeneration;
                    mapGeneration = value;
                    MapGenerationChanged?.Invoke(this, new ValueChangedEventArgs<MapGeneration>(oldValue, mapGeneration));
                }
            }
        }

        public MapConfiguration MapConfig
        {
            get => mapConfig;
            set
            {
                if (mapConfig != value)
                {
                    MapConfiguration oldValue = mapConfig;
                    mapConfig = value;
                    MapConfigChanged?.Invoke(this, new ValueChangedEventArgs<MapConfiguration>(oldValue, mapConfig));
                }
            }
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public void GenerateMap()
        {
            Map = MapGeneration.GenerateMap(MapConfig);
        }
        #endregion
    }
}
