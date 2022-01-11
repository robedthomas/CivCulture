using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using GenericUtilities;
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
        public const decimal MAX_RESOURCE_COST_MULTIPLIER = 3M;
        public const decimal MIN_RESOURCE_COST_MULTIPLIER = 0.1M;

        private decimal popGrowthProgress;
        private PopTemplate nextPopTemplate;
        private Terrain terrain;
        private Building currentConstruction;
        private Culture dominantCulture;
        #endregion

        #region Events
        public event ValueChangedEventHandler<decimal> PopGrowthProgressChanged;
        public event ValueChangedEventHandler<PopTemplate> NextPopTemplateChanged;
        public event ValueChangedEventHandler<Terrain> TerrainChanged;
        public event ValueChangedEventHandler<Building> CurrentConstructionChanged;
        public event ValueChangedEventHandler<Culture> DominantCultureChanged;
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

        public Culture DominantCulture
        {
            get => dominantCulture;
            set
            {
                if (dominantCulture != value)
                {
                    Culture oldValue = dominantCulture;
                    dominantCulture = value;
                    DominantCultureChanged?.Invoke(this, new ValueChangedEventArgs<Culture>(oldValue, value));
                }
            }
        }

        public ObservableCollection<Pop> Pops { get; protected set; }

        public ObservableCollection<Job> Jobs { get; protected set; }

        public ObservableCollection<Building> Buildings { get; protected set; }

        public ObservableCollection<BuildingTemplate> AvailableBuildings { get; protected set; }

        public ObservableCollection<TerrainResource> TerrainResources { get; protected set; }

        public Market ResourceMarket { get; protected set; }
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
            ResourceMarket = new Market(CalculateResourcePrice);
            EmptyBuildingSlotCount = BUILDING_SLOTS_PER_SPACE;

            Buildings.CollectionChanged += Buildings_CollectionChanged;
            Pops.CollectionChanged += Pops_CollectionChanged;
        }
        #endregion

        #region Methods
        public ConsumeablesCollection GetTotalOutput()
        {
            ConsumeablesCollection totalOutput = ConsumeablesCollection.Sum(
                from job in Jobs
                where job.Worker != null
                select job.Template.Outputs
                );
            totalOutput.Add(ConsumeablesCollection.Sum(
                from building in Buildings
                where building.IsComplete
                select building.Template.Outputs
                ));
            return totalOutput;
        }

        private decimal CalculateResourcePrice(Consumeable targetResource, Market owner)
        {
            decimal countSupplied = owner.SuppliedResources.ContainsKey(targetResource) ? owner.SuppliedResources[targetResource] : 0;
            decimal countDemanded = owner.DemandedResources.ContainsKey(targetResource) ? owner.DemandedResources[targetResource] : 0;
            return targetResource.BaseValue * CalculateResourcePriceMultiplier(countSupplied, countDemanded);
        }

        private static decimal CalculateResourcePriceMultiplier(decimal countSupplied, decimal countDemanded)
        {
            if (countSupplied == countDemanded) // Also covers both being zero
            {
                return 1;
            }
            else if (countSupplied == 0)
            {
                return MAX_RESOURCE_COST_MULTIPLIER;
            }
            else if (countDemanded == 0)
            {
                return MIN_RESOURCE_COST_MULTIPLIER;
            }
            else if (countSupplied > countDemanded)
            {
                return Math.Max(countDemanded / countSupplied, MIN_RESOURCE_COST_MULTIPLIER);
            }
            else
            {
                return Math.Min(countDemanded / countSupplied, MAX_RESOURCE_COST_MULTIPLIER);
            }
        }

        private Culture DetermineDominantCulture()
        {
            if (Pops.Count == 0)
            {
                return null;
            }
            Dictionary<Culture, int> countOfPopsOfCultures = new Dictionary<Culture, int>();
            foreach (Pop pop in Pops)
            {
                if (!countOfPopsOfCultures.ContainsKey(pop.Culture))
                {
                    countOfPopsOfCultures.Add(pop.Culture, 0);
                }
                countOfPopsOfCultures[pop.Culture]++;
            }
            int topCulturePopCount = countOfPopsOfCultures.Values.Max();
            IEnumerable<Culture> tiedCultures = countOfPopsOfCultures.Keys.Where(culture => countOfPopsOfCultures[culture] == topCulturePopCount);
            if (tiedCultures.Count() == 0)
            {
                return null;
            }
            else if (tiedCultures.Count() == 1)
            {
                return tiedCultures.First();
            }
            else
            {
                if (tiedCultures.Contains(DominantCulture))
                {
                    return DominantCulture;
                }
                else
                {
                    return tiedCultures.PickRandom(); // @TODO: use GameInstance's random seed
                }
            }
        }

        private void Buildings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Building newBuilding in e.NewItems)
                {
                    foreach (Job newJob in newBuilding.ChildJobs)
                    {
                        Jobs.Add(newJob);
                    }
                }
                EmptyBuildingSlotCount -= e.NewItems.Count;
            }
            if (e.OldItems != null)
            {
                foreach (Building oldBuilding in e.OldItems)
                {
                    foreach (Job oldJob in oldBuilding.ChildJobs)
                    {
                        Jobs.Remove(oldJob);
                    }
                }
                EmptyBuildingSlotCount += e.OldItems.Count;
            }
        }

        private void Pops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null || e.OldItems != null)
            {
                DominantCulture = DetermineDominantCulture();
            }
            if (e.NewItems != null)
            {
                foreach (Pop newPop in e.NewItems)
                {
                    newPop.CultureChanged += Pop_CultureChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Pop oldPop in e.OldItems)
                {
                    oldPop.CultureChanged -= Pop_CultureChanged;
                }
            }
        }

        private void Pop_CultureChanged(object sender, ValueChangedEventArgs<Culture> e)
        {
            DominantCulture = DetermineDominantCulture();
        }
        #endregion
    }
}
