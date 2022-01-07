using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class TerrainResourceTemplate : GameComponent
    {
        #region Static Members
        public static bool Initialized = false;

        public static TerrainResourceTemplate Wilderness;
        public static TerrainResourceTemplate Wheat;

        public static void InitializeTerrainResources()
        {
            JobTemplate.InitializeJobTemplates();
            Wilderness = new TerrainResourceTemplate() { Name = "Wilderness" };
            Wilderness.ChildJobTemplates.Add(JobTemplate.Gatherer_Wilderness);

            Wheat = new TerrainResourceTemplate() { Name = "Wheat" };
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
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
