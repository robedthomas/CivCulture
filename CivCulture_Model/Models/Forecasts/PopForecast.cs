using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Forecasts
{
    public class PopForecast : Forecast<Pop>
    {
        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        public DecimalModifiable MoneyChange { get; set; }

        public DecimalModifiable SatisfactionChange { get; set; }
        #endregion

        #region Constructors
        public PopForecast(Pop sourceModel) : base(sourceModel) { }
        #endregion

        #region Methods
        #endregion
    }
}
