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

        public static JobTemplate Builder;
        public static JobTemplate Gatherer_Wilderness;
        public static JobTemplate Gatherer_Wheat;
        public static JobTemplate Farmer_Wheat;

        public static void InitializeJobTemplates()
        {
            Resource.InitializeResources();
            Builder = new JobTemplate("Builder", 1, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Production, 1M } });

            Gatherer_Wilderness = new JobTemplate("Gatherer", 0, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Fundamental.Food, 1.2M } });

            Gatherer_Wheat = new JobTemplate("Gatherer (Wheat)", 0, 1, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Resource.Wheat, 1M } });
            Farmer_Wheat = new JobTemplate("Farmer (Wheat)", 0, 3, new ConsumeablesCollection() { }, new ConsumeablesCollection() { { Resource.Wheat, 2M } });
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
