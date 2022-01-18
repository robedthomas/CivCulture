using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Culture : GameComponent
    {
        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public Culture Parent { get; protected set; }

        public ObservableCollection<Culture> Children { get; protected set; }

        public ObservableCollection<Pop> PopsOfCulture { get; protected set; }

        public ObservableCollection<MapSpace> SpacesOfCulture { get; protected set; }

        public ObservableCollection<Technology> ResearchedTechnologies { get; protected set; }

        public ObservableCollection<Technology> AvailableTechnologies { get; protected set; }

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
                }
            }
            if (e.OldItems != null)
            {
                foreach (Technology oldTech in e.OldItems)
                {
                    TechModifiers.RemoveRange(oldTech.Template.Modifiers);
                }
            }
        }
        #endregion
    }
}
