using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Consumeable : GameComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; private set; }

        public decimal BaseValue { get; protected set; }

        public bool Accumulates { get; protected set; }

        public bool IsCultureWide { get; protected set; }
        #endregion

        #region Constructors
        public Consumeable(string name, decimal baseValue, bool accumulates = true, bool cultureWide = false)
        {
            Name = name;
            BaseValue = baseValue;
            Accumulates = accumulates;
            IsCultureWide = cultureWide;
        }
        #endregion

        #region Methods
        #endregion
    }
}
