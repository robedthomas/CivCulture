using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
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
    [DebuggerDisplay("{Name} Culture")]
    public class Culture : ResourceOwner, ITechResearcher, ITechModifiable
    {
        #region Events
        public ValueChangedEventHandler<Technology> CurrentResearchChanged;
        #endregion

        #region Fields
        private Technology currentResearch;
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public Culture Parent { get; protected set; }

        public ObservableCollection<Culture> Children { get; protected set; }

        public ObservableCollection<Pop> PopsOfCulture { get; protected set; }

        public ObservableCollection<MapSpace> SpacesOfCulture { get; protected set; }

        public ObservableCollection<BuildingTemplate> EnabledBuildings { get; protected set; }

        public ObservableCollection<Technology> ResearchedTechnologies { get; protected set; }

        public ObservableCollection<Technology> AvailableTechnologies { get; protected set; }

        public Technology CurrentResearch
        {
            get => currentResearch;
            set
            {
                if (currentResearch != value)
                {
                    Technology oldValue = currentResearch;
                    currentResearch = value;
                    CurrentResearchChanged?.Invoke(this, new ValueChangedEventArgs<Technology>(oldValue, value));
                }
            }
        }

        public TechModifierCollection TechModifiers { get; protected set; }

        public ITechResearcher TechSource => this;

        private Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler> ModifiersListHandlers { get; set; }
        #endregion

        #region Constructors
        public Culture(string name, Culture parent)
        {
            Name = name;
            Parent = parent;
            Children = new ObservableCollection<Culture>();
            PopsOfCulture = new ObservableCollection<Pop>();
            SpacesOfCulture = new ObservableCollection<MapSpace>();
            EnabledBuildings = new ObservableCollection<BuildingTemplate>();
            ResearchedTechnologies = new ObservableCollection<Technology>();
            AvailableTechnologies = new ObservableCollection<Technology>();
            TechModifiers = new TechModifierCollection();
            ModifiersListHandlers = new Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler>();

            TechModifiers.CollectionChanged += TechModifiers_CollectionChanged;
            ResearchedTechnologies.CollectionChanged += ResearchedTechnologies_CollectionChanged;
        }
        #endregion

        #region Methods
        private void ResearchedTechnologies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Technology newTech in e.NewItems)
                {
                    TechModifiers.AddRange(newTech.Template.Modifiers);
                    AvailableTechnologies.Remove(newTech);
                    MakeChildTechsAvailable(newTech);
                }
            }
            if (e.OldItems != null)
            {
                foreach (Technology oldTech in e.OldItems)
                {
                    TechModifiers.RemoveRange(oldTech.Template.Modifiers);
                    if (IsTechAvailable(oldTech.Template))
                    {
                        AvailableTechnologies.Add(oldTech);
                    }
                    MakeChildTechsUnavailable(oldTech);
                }
            }
        }

        private void TechModifiers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private bool IsTechAvailable(TechnologyTemplate tech)
        {
            // If ALL of a tech's parent techs have been researched by this culture, add it to the available techs
            if (tech.Parents.All(parentTech => ResearchedTechnologies.Any(researchedTech => researchedTech.Template == parentTech)))
            {
                return true;
            }
            return false;
        }

        private void MakeChildTechsAvailable(Technology newTech)
        {
            foreach (TechnologyTemplate childTech in newTech.Template.Children)
            {
                // If ALL of a tech's parent techs have been researched by this culture, add it to the available techs
                if (IsTechAvailable(childTech))
                {
                    AvailableTechnologies.Add(new Technology(childTech, this));
                }
            }
        }

        private void MakeChildTechsUnavailable(Technology oldTech)
        {
            // Remove ALL children of this tech from the available techs
            foreach (TechnologyTemplate childTech in oldTech.Template.Children)
            {
                Technology targetTech = AvailableTechnologies.FirstOrDefault(tech => tech.Template == childTech);
                if (targetTech != null)
                {
                    AvailableTechnologies.Remove(targetTech);
                }
            }
        }

        private void ApplyModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, TechModifier<decimal> modifier)
        {
            switch (modifierKey.Item1)
            {
                case StatModification.CultureEnableBuilding:
                    if (!EnabledBuildings.Contains(modifierKey.Item2 as BuildingTemplate))
                    {
                        EnabledBuildings.Add(modifierKey.Item2 as BuildingTemplate);
                    }
                    break;
            }
            // @TODO: handle other types of StatModification
        }

        private void UnapplyModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, TechModifier<decimal> modifier)
        {
            switch (modifierKey.Item1)
            {
                case StatModification.CultureEnableBuilding:
                    if (EnabledBuildings.Contains(modifierKey.Item2 as BuildingTemplate))
                    {
                        EnabledBuildings.Remove(modifierKey.Item2 as BuildingTemplate);
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
            NotifyCollectionChangedEventHandler newHandler = GetTechModifierListChangedHandler(modifierKey);
            if (modifierKey.Item1 == StatModification.CultureEnableBuilding)
            {
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
            if (modifierKey.Item1 == StatModification.CultureEnableBuilding)
            {
                foreach (TechModifier<decimal> mod in modifierCollection)
                {
                    UnapplyModifier(modifierKey, mod);
                }
                modifierCollection.CollectionChanged -= ModifiersListHandlers[modifierKey];
                ModifiersListHandlers.Remove(modifierKey);
                return true;
            }
            return false;
        }
        #endregion
    }
}
