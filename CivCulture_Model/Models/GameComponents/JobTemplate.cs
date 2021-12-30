using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class JobTemplate : GameComponent
    {
        #region Static Members
        public const int MAX_JOB_PRIORITY = 10;
        public const int UNEMPLOYED_JOB_PRIORITY = MAX_JOB_PRIORITY;

        public static JobTemplate Gatherer_Wilderness;
        public static JobTemplate Gatherer_Wheat;
        public static JobTemplate Farmer_Wheat;

        public static void InitializeJobTemplates()
        {
            Resource.InitializeResources();
            Gatherer_Wilderness = new JobTemplate("Gatherer", 0, null, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Food, 1.2M } });

            Gatherer_Wheat = new JobTemplate("Gatherer (Wheat)", 0, null, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Resource.Wheat, 1M } });
            Farmer_Wheat = new JobTemplate("Farmer (Wheat)", 0, null, 3, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Resource.Wheat, 2M } });
        }

        public static void InitializeTerrainResourceBindings()
        {
            Gatherer_Wilderness.Source = TerrainResource.Wilderness;

            Gatherer_Wheat.Source = TerrainResource.Wheat;
            Farmer_Wheat.Source = TerrainResource.Wheat;
        }
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public int Priority { get; protected set; }

        public JobSource Source { get; protected set; }

        public decimal BasePay { get; protected set; }

        public ConsumeablesCollection Inputs { get; protected set; }

        public ConsumeablesCollection Outputs { get; protected set; }
        #endregion

        #region Constructors
        public JobTemplate(string name, int priority, JobSource source, int basePay, ConsumeablesCollection inputs = null, ConsumeablesCollection outputs = null)
        {
            Name = name;
            Priority = priority;
            Source = source;
            BasePay = basePay;
            Inputs = new ConsumeablesCollection();
            if (inputs != null)
            {
                foreach (KeyValuePair<Consumeable, decimal> pair in inputs)
                {
                    Inputs.Add(pair);
                }
            }
            Outputs = new ConsumeablesCollection();
            if (outputs != null)
            {
                foreach (KeyValuePair<Consumeable, decimal> pair in outputs)
                {
                    Outputs.Add(pair);
                }
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
