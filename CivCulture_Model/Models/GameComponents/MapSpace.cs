using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class MapSpace : GameComponent
    {
        #region Fields
        private decimal popGrowthProgress;
        private PopTemplate nextPopTemplate;
        private Terrain terrain;
        private ConsumeablesCollection resourceStockpile;
        private decimal resourceStockpileMoney;
        #endregion

        #region Events
        public event ValueChangedEventHandler<decimal> PopGrowthProgressChanged;
        public event ValueChangedEventHandler<PopTemplate> NextPopTemplateChanged;
        public event ValueChangedEventHandler<Terrain> TerrainChanged;
        public event ValueChangedEventHandler<ConsumeablesCollection> ResourceStockpileChanged;
        public event ValueChangedEventHandler<decimal> ResourceStockpileMoneyChanged;
        #endregion

        #region Properties
        public int Row { get; protected set; }

        public int Column { get; protected set; }

        public string Name { get; set; }

        public decimal PopGrowthProgress
        {
            get => popGrowthProgress;
            set
            {
                if (popGrowthProgress != value)
                {
                    decimal oldValue = popGrowthProgress;
                    popGrowthProgress = value;
                    PopGrowthProgressChanged?.Invoke(this, new ValueChangedEventArgs<decimal>(oldValue, popGrowthProgress));
                }
            }
        }

        public PopTemplate NextPopTemplate
        {
            get => nextPopTemplate;
            set
            {
                if (nextPopTemplate != value)
                {
                    PopTemplate oldValue = nextPopTemplate;
                    nextPopTemplate = value;
                    NextPopTemplateChanged?.Invoke(this, new ValueChangedEventArgs<PopTemplate>(oldValue, nextPopTemplate));
                }
            }
        }

        public Terrain Terrain
        {
            get => terrain;
            set
            {
                if (terrain != value)
                {
                    Terrain oldValue = terrain;
                    terrain = value;
                    TerrainChanged?.Invoke(this, new ValueChangedEventArgs<Terrain>(oldValue, terrain));
                }
            }
        }

        public ConsumeablesCollection ResourceStockpile
        {
            get => resourceStockpile;
            set
            {
                if (resourceStockpile != value)
                {
                    ConsumeablesCollection oldValue = resourceStockpile;
                    resourceStockpile = value;
                    ResourceStockpileChanged?.Invoke(this, new ValueChangedEventArgs<ConsumeablesCollection>(oldValue, value));
                }
            }
        }

        public decimal ResourceStockpileMoney
        {
            get => resourceStockpileMoney;
            set
            {
                if (resourceStockpileMoney != value)
                {
                    decimal oldValue = resourceStockpileMoney;
                    resourceStockpileMoney = value;
                    ResourceStockpileMoneyChanged?.Invoke(this, new ValueChangedEventArgs<decimal>(oldValue, value));
                }
            }
        }

        public ObservableCollection<Pop> Pops { get; protected set; }

        public ObservableCollection<Job> Jobs { get; protected set; }

        public ObservableCollection<TerrainResource> TerrainResources { get; protected set; }
        #endregion

        #region Constructors
        public MapSpace(int row, int column, Terrain terrain, params TerrainResource[] terrainResources)
        {
            Row = row;
            Column = column;
            Terrain = terrain;
            Pops = new ObservableCollection<Pop>();
            Jobs = new ObservableCollection<Job>();
            TerrainResources = new ObservableCollection<TerrainResource>(terrainResources);
            ResourceStockpile = new ConsumeablesCollection();
        }
        #endregion

        #region Methods
        #endregion
    }
}
