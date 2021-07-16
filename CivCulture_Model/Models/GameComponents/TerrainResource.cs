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
        public static TerrainResource Wilderness;
        public static TerrainResource Wheat;

        static TerrainResource()
        {
            Job.InitializeJobs();
            Wilderness = new TerrainResource() { Name = "Wilderness" };
            Wilderness.ChildJobs.Add(Job.Gatherer_Wilderness);

            Wheat = new TerrainResource() { Name = "Wheat" };
            Wheat.ChildJobs.Add(Job.Gatherer_Wheat);
            Wheat.ChildJobs.Add(Job.Farmer_Wheat);
            Job.InitializeTerrainResourceBindings();
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
