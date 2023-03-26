using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class BuildingSlotTemplate : ComponentTemplate
    {
        #region Static Members
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public ObservableCollection<JobTemplate> ChildJobTemplates { get; protected set; }

        public ConsumeablesCollection ResourcesUponRemoval { get; protected set; }

        public Dictionary<Terrain, decimal> ProbabilityWeightPerTerrainType { get; protected set; }

        public Dictionary<Terrain, BuildingSlotTemplate> UnderlyingSlotType { get; protected set; }
        #endregion

        #region Constructors
        public BuildingSlotTemplate(string name, IEnumerable<JobTemplate> childJobTemplates, IDictionary<Consumeable, decimal> resourcesUponRemoval, IDictionary<Terrain, decimal> probabilityWeightPerTerrainType)
        {
            Name = name;
            ChildJobTemplates = new ObservableCollection<JobTemplate>(childJobTemplates);
            ResourcesUponRemoval = new ConsumeablesCollection(resourcesUponRemoval);
            ProbabilityWeightPerTerrainType = new Dictionary<Terrain, decimal>(probabilityWeightPerTerrainType);
            UnderlyingSlotType = new Dictionary<Terrain, BuildingSlotTemplate>();
        }
        #endregion

        #region Methods
        #endregion
    }
}
