using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class JobTemplate : ComponentTemplate
    {
        #region Static Members
        public const int MAX_JOB_PRIORITY = 10;
        public const int UNEMPLOYED_JOB_PRIORITY = MAX_JOB_PRIORITY;

        public static JobTemplate Builder;
        public static JobTemplate Gatherer_Wilderness;
        public static JobTemplate Farmer_Wilderness;
        public static JobTemplate Gatherer_Wheat;
        public static JobTemplate Farmer_Wheat;

        public static void InitializeJobTemplates()
        {
            Resource.InitializeResources();
            Builder = new JobTemplate("Builder", 1, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Production, 1M } });

            Gatherer_Wilderness = new JobTemplate("Gatherer", 0, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Food, 1.2M } });
            Farmer_Wilderness = new JobTemplate("Farmer", 0, 3, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Food, 1.7M } });

            Gatherer_Wheat = new JobTemplate("Gatherer (Wheat)", 0, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Food, 1.5M } });
            Farmer_Wheat = new JobTemplate("Farmer (Wheat)", 0, 3, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Food, 2M } });
        }
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public int Priority { get; protected set; }

        public decimal BasePay { get; protected set; }

        public ConsumeablesCollection Inputs { get; protected set; }

        public ConsumeablesCollection Outputs { get; protected set; }
        #endregion

        #region Constructors
        public JobTemplate(string name, int priority, int basePay, ConsumeablesCollection inputs = null, ConsumeablesCollection outputs = null)
        {
            Name = name;
            Priority = priority;
            BasePay = basePay;
            Inputs = new ConsumeablesCollection(inputs);
            Outputs = new ConsumeablesCollection(outputs);
        }
        #endregion

        #region Methods
        #endregion
    }
}
