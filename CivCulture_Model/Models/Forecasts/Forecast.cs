using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Forecasts
{
    public abstract class Forecast<TModel>
    {
        public event ValueChangedEventHandler<TModel> SourceModelChanged;

        protected TModel sourceModel;

        public TModel SourceModel
        {
            get => sourceModel;
            set
            {
                if ((sourceModel == null && value != null) || sourceModel.Equals(value))
                {
                    TModel oldValue = sourceModel;
                    sourceModel = value;
                    SourceModelChanged?.Invoke(this, new ValueChangedEventArgs<TModel>(oldValue, value));
                }
            }
        }

        public Forecast(TModel sourceModel)
        {
            SourceModel = sourceModel;
        }
    }
}
