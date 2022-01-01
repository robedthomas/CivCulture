using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class TerrainResource : JobSource
    {
        #region Static Members
        public static bool Initialized = false;

        public static TerrainResource Wilderness;
        public static TerrainResource Wheat;

        public static void InitializeTerrainResources()
        {
            JobTemplate.InitializeJobTemplates();
            Wilderness = new TerrainResource() { Name = "Wilderness" };
            Wilderness.ChildJobs.Add(JobTemplate.Gatherer_Wilderness);

            Wheat = new TerrainResource() { Name = "Wheat" };
            Wheat.ChildJobs.Add(JobTemplate.Gatherer_Wheat);
            Wheat.ChildJobs.Add(JobTemplate.Farmer_Wheat);
            Initialized = true;
        }
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
