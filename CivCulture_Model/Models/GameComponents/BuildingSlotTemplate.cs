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
        public static bool Initialized = false;

        public static BuildingSlotTemplate Grassland;
        public static BuildingSlotTemplate Wilderness;
        public static BuildingSlotTemplate Wheat;

        public static void InitializeBuildingSlotTemplates()
        {
            JobTemplate.InitializeJobTemplates();
            Wilderness = new BuildingSlotTemplate(new Dictionary<Consumeable, decimal>() { { Fundamental.Food, 20 } }) { Name = "Wilderness" };
            Wilderness.ChildJobTemplates.Add(JobTemplate.Gatherer_Wilderness);

            Wheat = new BuildingSlotTemplate(new Dictionary<Consumeable, decimal>() { { Fundamental.Food, 40 } }) { Name = "Wheat" };
            Wheat.ChildJobTemplates.Add(JobTemplate.Gatherer_Wheat);
            Initialized = true;
        }
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public ObservableCollection<JobTemplate> ChildJobTemplates { get; protected set; } = new ObservableCollection<JobTemplate>();

        public ReadOnlyDictionary<Consumeable, decimal> ResourcesUponRemoval { get; protected set; }
        #endregion

        #region Constructors
        public BuildingSlotTemplate(IDictionary<Consumeable, decimal> resourcesUponRemoval)
        {
            ResourcesUponRemoval = new ReadOnlyDictionary<Resource, decimal>(resourcesUponRemoval);
        }
        #endregion

        #region Methods
        #endregion
    }
}
