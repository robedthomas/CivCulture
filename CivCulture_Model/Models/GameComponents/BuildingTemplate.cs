using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class BuildingTemplate : ComponentTemplate
    {
        #region Static Members
        #endregion

        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public string DisplayName { get; protected set; }

        public bool IsSpaceUnique { get; protected set; }

        public ConsumeablesCollection Costs { get; protected set; }

        public ConsumeablesCollection Outputs { get; protected set; }

        public ObservableCollection<JobTemplate> Jobs { get; protected set; }

        public ObservableCollection<BuildingSlotTemplate> RequisitBuildingSlots { get; protected set; }
        #endregion

        #region Constructors
        public BuildingTemplate(string name, string displayName, IEnumerable<JobTemplate> jobs, IEnumerable<BuildingSlotTemplate> requisitBuildingSlots, bool isSpaceUnique = false, ConsumeablesCollection costs = null, ConsumeablesCollection outputs = null)
        {
            Name = name;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? name : displayName;
            Costs = new ConsumeablesCollection(costs);
            Outputs = new ConsumeablesCollection(outputs);
            Jobs = new ObservableCollection<JobTemplate>(jobs);
            RequisitBuildingSlots = new ObservableCollection<BuildingSlotTemplate>(requisitBuildingSlots);
        }
        #endregion

        #region Methods
        #endregion
    }
}
