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
        #endregion

        #region Constructors
        public Culture(string name, Culture parent)
        {
            Name = name;
            Parent = parent;
            Children = new ObservableCollection<Culture>();
            PopsOfCulture = new ObservableCollection<Pop>();
            SpacesOfCulture = new ObservableCollection<MapSpace>();
        }
        #endregion

        #region Methods
        #endregion
    }
}
