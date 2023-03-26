using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using GenericUtilities;
using GenericUtilities.Observables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("MapSpace: (X,Y)=({Column},{Row}) Terrain={Terrain.Name}")]
    public class MapSpace : ResourceOwner, ITechModifiable
    {
        #region Fields
        public const int BUILDING_SLOTS_PER_SPACE = 3;
        public const decimal MAX_RESOURCE_COST_MULTIPLIER = 3M;
        public const decimal MIN_RESOURCE_COST_MULTIPLIER = 0.1M;
        public const decimal BASE_PRODUCTION_THROUGHPUT = 3M;

        private decimal popGrowthProgress;
        private decimal productionThroughput;
        private PopTemplate nextPopTemplate;
        private Terrain terrain;
        private Building currentConstruction;
        private Culture dominantCulture;
        #endregion

        #region Events
        public event ValueChangedEventHandler<decimal> PopGrowthProgressChanged;
        public event ValueChangedEventHandler<decimal> ProductionThroughputChanged;
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

        /// <summary>
        /// The maximum amount of Production that can be used per turn to advance this space's construction
        /// </summary>
        public decimal ProductionThroughput
        {
            get => productionThroughput;
            set
            {
                if (productionThroughput != value)
                {
                    decimal oldValue = productionThroughput;
                    productionThroughput = value;
                    ProductionThroughputChanged?.Invoke(this, new ValueChangedEventArgs<decimal>(oldValue, value));
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

        public ITechResearcher TechSource
        {
            get => DominantCulture;
        }

        public TechModifierCollection TechModifiers { get; private set; }

        private Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler> ModifiersListHandlers { get; set; }

        public ObservableCollection<Pop> Pops { get; protected set; }

        public ObservableCollection<Job> Jobs { get; protected set; }

        private ObservableDictionary<Technology, List<Job>> JobsFromTech { get; set; }

        public ObservableCollection<Building> Buildings { get; protected set; }

        public ObservableCollection<BuildingTemplate> AvailableBuildings { get; protected set; }

        public ObservableCollection<BuildingSlot> BuildingSlots { get; protected set; }

        public Market ResourceMarket { get; protected set; }
        #endregion

        #region Constructors
        public MapSpace(int row, int column, Terrain terrain) : base()
        {
            Row = row;
            Column = column;
            Terrain = terrain;
            ProductionThroughput = BASE_PRODUCTION_THROUGHPUT;
            TechModifiers = new TechModifierCollection();
            ModifiersListHandlers = new Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler>();
            Pops = new ObservableCollection<Pop>();
            Jobs = new ObservableCollection<Job>();
            JobsFromTech = new ObservableDictionary<Technology, List<Job>>();
            Buildings = new ObservableCollection<Building>();
            AvailableBuildings = new ObservableCollection<BuildingTemplate>();
            BuildingSlots = new ObservableCollection<BuildingSlot>();
            ResourceMarket = new Market(CalculateResourcePrice);
            EmptyBuildingSlotCount = BUILDING_SLOTS_PER_SPACE;

            Buildings.CollectionChanged += Buildings_CollectionChanged;
            Pops.CollectionChanged += Pops_CollectionChanged;
            JobsFromTech.CollectionChanged += JobsFromTech_CollectionChanged;
            DominantCultureChanged += This_DominantCultureChanged;
        }
        #endregion

        #region Methods
        public ConsumeablesCollection GetTotalOutput()
        {
            ConsumeablesCollection totalOutput = ConsumeablesCollection.Sum(
                from job in Jobs
                where job.Worker != null
                select job.Outputs
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

        private void ApplyModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, TechModifier<decimal> modifier)
        {
            switch (modifierKey.Item1)
            {
                case StatModification.SpaceProductionThroughput:
                    ProductionThroughput += modifier.Modification;
                    break;
                case StatModification.SpaceJobs:
                    Technology targetTech = DominantCulture.ResearchedTechnologies.First(tech => tech.Template == modifier.Technology);
                    if (modifier.Modification > 0)
                    {
                        List<Job> newJobs = new List<Job>();
                        for (int i = 0; i < modifier.Modification; i++)
                        {
                            newJobs.Add(new Job(modifierKey.Item2 as JobTemplate, targetTech));
                        }
                        JobsFromTech.Add(targetTech, newJobs);
                    }
                    else if (modifier.Modification < 0)
                    {
                        throw new InvalidOperationException("Cannot remove jobs from space via new technology");
                    }
                    break;
            }
            // @TODO: handle other types of StatModification
        }

        private void UnapplyModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, TechModifier<decimal> modifier)
        {
            switch (modifierKey.Item1)
            {
                case StatModification.SpaceProductionThroughput:
                    ProductionThroughput -= modifier.Modification;
                    break;
                case StatModification.SpaceJobs:
                    Technology targetTech = JobsFromTech.Keys.First(tech => tech.Template == modifier.Technology);
                    if (modifier.Modification > 0)
                    {
                        JobsFromTech.Remove(targetTech);
                    }
                    else if (modifier.Modification < 0)
                    {
                        throw new InvalidOperationException("Cannot remove jobs from space via new technology");
                    }
                    break;
            }
            // @TODO: handle other types of StatModification
        }

        private NotifyCollectionChangedEventHandler GetTechModifierListChangedHandler(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey)
        {
            return new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (TechModifier<decimal> newMod in e.NewItems)
                    {
                        ApplyModifier(modifierKey, newMod);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (TechModifier<decimal> oldMod in e.OldItems)
                    {
                        UnapplyModifier(modifierKey, oldMod);
                    }
                }
            });
        }

        private bool TryAddTechModifierList(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, ObservableCollection<TechModifier<decimal>> modifierCollection)
        {
            if (modifierKey.Item1 == StatModification.SpaceProductionThroughput || modifierKey.Item1 == StatModification.SpaceJobs)
            {
                NotifyCollectionChangedEventHandler newHandler = GetTechModifierListChangedHandler(modifierKey);
                TechModifiers.Add(modifierKey, modifierCollection);
                modifierCollection.CollectionChanged += newHandler;
                ModifiersListHandlers.Add(modifierKey, newHandler);
                foreach (TechModifier<decimal> mod in modifierCollection)
                {
                    ApplyModifier(modifierKey, mod);
                }
                return true;
            }
            return false;
        }

        private bool TryRemoveTechModifierList(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, ObservableCollection<TechModifier<decimal>> modifierCollection)
        {
            if (TechModifiers.ContainsKey(modifierKey) && TechModifiers[modifierKey] == modifierCollection)
            {
                foreach (TechModifier<decimal> mod in modifierCollection)
                {
                    UnapplyModifier(modifierKey, mod);
                }
                modifierCollection.CollectionChanged -= ModifiersListHandlers[modifierKey];
                TechModifiers.Remove(modifierKey);
                ModifiersListHandlers.Remove(modifierKey);
                return true;
            }
            return false;
        }

        private void This_DominantCultureChanged(object sender, ValueChangedEventArgs<Culture> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.EnabledBuildings.CollectionChanged -= DominantCulture_EnabledBuildings_CollectionChanged;
                e.OldValue.TechModifiers.CollectionChanged -= Culture_TechModifiers_CollectionChanged;
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<TechModifier<decimal>>> modifierPair in e.OldValue.TechModifiers)
                {
                    TryRemoveTechModifierList(modifierPair.Key, modifierPair.Value);
                }
                AvailableBuildings.Clear();
            }
            if (e.NewValue != null)
            {
                e.NewValue.EnabledBuildings.CollectionChanged += DominantCulture_EnabledBuildings_CollectionChanged;
                e.NewValue.TechModifiers.CollectionChanged += Culture_TechModifiers_CollectionChanged;
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<TechModifier<decimal>>> modifierPair in e.NewValue.TechModifiers)
                {
                    TryAddTechModifierList(modifierPair.Key, modifierPair.Value);
                }
                foreach (BuildingTemplate building in e.NewValue.EnabledBuildings)
                {
                    if (!building.IsSpaceUnique || !Buildings.Any(b => b.Template == building))
                    {
                        AvailableBuildings.Add(building);
                    }
                }
            }
        }

        private void DominantCulture_EnabledBuildings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BuildingTemplate newBuilding in e.NewItems)
                {
                    if (!AvailableBuildings.Contains(newBuilding))
                    {
                        AvailableBuildings.Add(newBuilding);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (BuildingTemplate oldBuilding in e.OldItems)
                {
                    if (AvailableBuildings.Contains(oldBuilding))
                    {
                        AvailableBuildings.Remove(oldBuilding);
                    }
                }
            }
        }

        private void Culture_TechModifiers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<TechModifier<decimal>>> newPair in e.NewItems)
                {
                    TryAddTechModifierList(newPair.Key, newPair.Value);
                }
            }
            if (e.OldItems != null)
            {
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<TechModifier<decimal>>> oldPair in e.OldItems)
                {
                    TryRemoveTechModifierList(oldPair.Key, oldPair.Value);
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

        private void JobsFromTech_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (KeyValuePair<Technology, List<Job>> newPair in e.NewItems)
                {
                    foreach (Job newJob in newPair.Value)
                    {
                        Jobs.Add(newJob);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (KeyValuePair<Technology, List<Job>> oldPair in e.OldItems)
                {
                    foreach (Job oldJob in oldPair.Value)
                    {
                        Jobs.Remove(oldJob);
                    }
                }
            }
        }
        #endregion
    }
}
