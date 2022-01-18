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
    public class Culture : ResourceOwner
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
        #endregion

        #region Constructors
        public Culture(string name, Culture parent)
        {
            Name = name;
            Parent = parent;
            Children = new ObservableCollection<Culture>();
            PopsOfCulture = new ObservableCollection<Pop>();
            SpacesOfCulture = new ObservableCollection<MapSpace>();
            ResearchedTechnologies = new ObservableCollection<Technology>();
            AvailableTechnologies = new ObservableCollection<Technology>();
            TechModifiers = new TechModifierCollection();

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
        #endregion
    }
}
