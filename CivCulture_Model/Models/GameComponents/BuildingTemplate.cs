using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class BuildingTemplate : GameComponent
    {
        #region Static Members
        public static BuildingTemplate MudHuts;

        public static void InitializeBuildingTemplates()
        {
            MudHuts = new BuildingTemplate("Mud Huts", new ObservableCollection<JobTemplate>(), false, new ConsumeablesCollection() { { Fundamental.Production, 10 } }, new ConsumeablesCollection() { { Fundamental.Shelter, 3 } });
        }
        #endregion

        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public bool IsSpaceUnique { get; protected set; }

        public ConsumeablesCollection Costs { get; protected set; }

        public ConsumeablesCollection Outputs { get; protected set; }

        public ObservableCollection<JobTemplate> Jobs { get; protected set; }
        #endregion

        #region Constructors
        public BuildingTemplate(string name, IEnumerable<JobTemplate> jobs, bool isSpaceUnique = false, ConsumeablesCollection costs = null, ConsumeablesCollection outputs = null)
        {
            Name = name;
            Costs = new ConsumeablesCollection(costs);
            Outputs = new ConsumeablesCollection(outputs);
            Jobs = new ObservableCollection<JobTemplate>(jobs);
        }
        #endregion

        #region Methods
        #endregion
    }
}
