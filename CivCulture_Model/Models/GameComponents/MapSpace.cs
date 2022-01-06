using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("MapSpace: (X,Y)=({Column},{Row}) Terrain={Terrain.Name}")]
    public class MapSpace : ResourceOwner
    {
        #region Fields
        public const int BUILDING_SLOTS_PER_SPACE = 10;

        private decimal popGrowthProgress;
        private PopTemplate nextPopTemplate;
        private Terrain terrain;
        private Building currentConstruction;
        #endregion

        #region Events
        public event ValueChangedEventHandler<decimal> PopGrowthProgressChanged;
        public event ValueChangedEventHandler<PopTemplate> NextPopTemplateChanged;
        public event ValueChangedEventHandler<Terrain> TerrainChanged;
        public event ValueChangedEventHandler<Building> CurrentConstructionChanged;
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

        public int EmptyBuildingSlotCount { get; protected set; }

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

        public Building CurrentConstruction
        {
            get => currentConstruction;
            set
            {
                if (currentConstruction != value)
                {
                    Building oldValue = currentConstruction;
                    currentConstruction = value;
                    CurrentConstructionChanged?.Invoke(this, new ValueChangedEventArgs<Building>(oldValue, value));
                }
            }
        }

        public ObservableCollection<Pop> Pops { get; protected set; }

        public ObservableCollection<Job> Jobs { get; protected set; }

        public ObservableCollection<Building> Buildings { get; protected set; }

        public ObservableCollection<BuildingTemplate> AvailableBuildings { get; protected set; }

        public ObservableCollection<TerrainResource> TerrainResources { get; protected set; }
        #endregion

        #region Constructors
        public MapSpace(int row, int column, Terrain terrain, params TerrainResource[] terrainResources) : base()
        {
            Row = row;
            Column = column;
            Terrain = terrain;
            Pops = new ObservableCollection<Pop>();
            Jobs = new ObservableCollection<Job>();
            Buildings = new ObservableCollection<Building>();
            AvailableBuildings = new ObservableCollection<BuildingTemplate>();
            TerrainResources = new ObservableCollection<TerrainResource>(terrainResources);
            EmptyBuildingSlotCount = BUILDING_SLOTS_PER_SPACE;

            Buildings.CollectionChanged += Buildings_CollectionChanged;
        }

        private void Buildings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                EmptyBuildingSlotCount -= e.NewItems.Count;
            }
            if (e.OldItems != null)
            {
                EmptyBuildingSlotCount += e.OldItems.Count;
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
