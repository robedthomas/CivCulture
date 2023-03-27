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

        public static JobTemplate ALL = new JobTemplate("ALL", null, -1, -1, null);
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public string DisplayName { get; protected set; }

        public int Priority { get; protected set; }

        public decimal BasePay { get; protected set; }

        public ConsumeablesCollection Inputs { get; protected set; }

        public ConsumeablesCollection Outputs { get; protected set; }

        public PopTemplate WorkerTemplate { get; protected set; }
        #endregion

        #region Constructors
        public JobTemplate(string name, string displayName, int priority, int basePay, PopTemplate workerTemplate, ConsumeablesCollection inputs = null, ConsumeablesCollection outputs = null)
        {
            Name = name;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? name : displayName;
            Priority = priority;
            BasePay = basePay;
            if (inputs is null)
            {
                inputs = new ConsumeablesCollection();
            }
            if (outputs is null)
            {
                outputs = new ConsumeablesCollection();
            }
            WorkerTemplate = workerTemplate;
            Inputs = new ConsumeablesCollection(inputs);
            Outputs = new ConsumeablesCollection(outputs);
        }
        #endregion

        #region Methods
        #endregion
    }
}
