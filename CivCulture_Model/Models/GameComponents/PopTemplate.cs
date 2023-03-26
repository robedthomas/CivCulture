using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class PopTemplate : ComponentTemplate
    {
        #region Static Members
        public static PopTemplate ALL = new PopTemplate("ALL");
        #endregion

        #region Fields
        private string name;
        private decimal progressFromSatisfactionRatio;
        private NeedCollection needs;
        #endregion

        #region Events
        public ValueChangedEventHandler<string> NameChanged;
        public ValueChangedEventHandler<decimal> ProgressFromSatisfactionRatioChanged;
        public ValueChangedEventHandler<NeedCollection> NeedsChanged;
        #endregion

        #region Properties
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    string oldValue = name;
                    name = value;
                    NameChanged?.Invoke(this, new ValueChangedEventArgs<string>(oldValue, value));
                }
            }
        }

        public decimal ProgressFromSatisfactionRatio
        {
            get => progressFromSatisfactionRatio;
            set
            {
                if (progressFromSatisfactionRatio != value)
                {
                    decimal oldValue = progressFromSatisfactionRatio;
                    progressFromSatisfactionRatio = value;
                    ProgressFromSatisfactionRatioChanged?.Invoke(this, new ValueChangedEventArgs<decimal>(oldValue, value));
                }
            }
        }

        public NeedCollection Needs
        {
            get => needs;
            protected set
            {
                if (needs != value)
                {
                    NeedCollection oldNeeds = needs;
                    needs = value;
                    NeedsChanged?.Invoke(this, new ValueChangedEventArgs<NeedCollection>(oldNeeds, needs));
                }
            }
        }
        #endregion

        #region Constructors
        public PopTemplate(string name, ConsumeablesCollection necessities = null, ConsumeablesCollection comforts = null, ConsumeablesCollection luxuries = null)
        {
            Name = name;
            ProgressFromSatisfactionRatio = 0.1M; 
            Needs = new NeedCollection();
            if (necessities != null)
            {
                Needs[NeedType.Necessity] = necessities;
            }
            if (comforts != null)
            {
                Needs[NeedType.Comfort] = comforts;
            }
            if (luxuries != null)
            {
                Needs[NeedType.Luxury] = luxuries;
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
