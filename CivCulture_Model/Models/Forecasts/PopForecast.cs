using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private ValueChangedEventHandler<PopTemplate> templateChangedHandler;
        private ValueChangedEventHandler<Job> jobChangedHandler;
        private ValueChangedEventHandler<MapSpace> spaceChangedHandler;
        private ValueChangedEventHandler<decimal> moneyChangedHandler;
        private ValueChangedEventHandler<decimal> satisfactionChangedHandler;
        private NotifyCollectionChangedEventHandler ownedResourcesCollectionChangedHandler;
        #endregion

        #region Properties
        public DecimalModifiable MoneyChange { get; private set; }

        public DecimalModifiable SatisfactionChange { get; private set; }

        public ValueChangedEventHandler<PopTemplate> TemplateChangedHandler
        {
            get => templateChangedHandler;
            set
            {
                if (templateChangedHandler != value)
                {
                    templateChangedHandler = value;
                    if (SourceModel != null)
                    {
                        SourceModel.TemplateChanged += templateChangedHandler;
                    }
                }
            }
        }

        public ValueChangedEventHandler<Job> JobChangedHandler
        {
            get => jobChangedHandler;
            set
            {
                if (jobChangedHandler != value)
                {
                    jobChangedHandler = value;
                    if (SourceModel != null)
                    {
                        SourceModel.JobChanged += jobChangedHandler;
                    }
                }
            }
        }

        public ValueChangedEventHandler<MapSpace> SpaceChangedHandler
        {
            get => spaceChangedHandler;
            set
            {
                if (spaceChangedHandler != value)
                {
                    spaceChangedHandler = value;
                    if (SourceModel != null)
                    {
                        SourceModel.SpaceChanged += spaceChangedHandler;
                    }
                }
            }
        }

        public ValueChangedEventHandler<decimal> MoneyChangedHandler
        {
            get => moneyChangedHandler;
            set
            {
                if (moneyChangedHandler != value)
                {
                    moneyChangedHandler = value;
                    if (SourceModel != null)
                    {
                        SourceModel.MoneyChanged += moneyChangedHandler;
                    }
                }
            }
        }

        public ValueChangedEventHandler<decimal> SatisfactionChangedHandler
        {
            get => satisfactionChangedHandler;
            set
            {
                if (satisfactionChangedHandler != value)
                {
                    satisfactionChangedHandler = value;
                    if (SourceModel != null)
                    {
                        SourceModel.SatisfactionChanged += satisfactionChangedHandler;
                    }
                }
            }
        }

        public NotifyCollectionChangedEventHandler OwnedResourcesCollectionChangedHandler
        {
            get => ownedResourcesCollectionChangedHandler;
            set
            {
                if (ownedResourcesCollectionChangedHandler != value)
                {
                    ownedResourcesCollectionChangedHandler = value;
                    if (SourceModel != null)
                    {
                        SourceModel.OwnedResources.CollectionChanged += ownedResourcesCollectionChangedHandler;
                    }
                }
            }
        }
        #endregion

        #region Constructors
        public PopForecast(Pop sourceModel) : base(sourceModel) 
        {
            MoneyChange = new DecimalModifiable();
            SatisfactionChange = new DecimalModifiable();
            SourceModelChanged += PopForecast_SourceModelChanged;
            PopForecast_SourceModelChanged(this, new ValueChangedEventArgs<Pop>(null, SourceModel));
        }
        #endregion

        #region Methods
        private void PopForecast_SourceModelChanged(object sender, Events.ValueChangedEventArgs<Pop> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.TemplateChanged -= TemplateChangedHandler;
                e.OldValue.JobChanged -= JobChangedHandler;
                e.OldValue.SpaceChanged -= SpaceChangedHandler;
                e.OldValue.MoneyChanged -= MoneyChangedHandler;
                e.OldValue.SatisfactionChanged -= SatisfactionChangedHandler;
                e.OldValue.OwnedResourcesChanged -= SourceModel_OwnedResourcesChanged;
                e.OldValue.OwnedResources.CollectionChanged -= OwnedResourcesCollectionChangedHandler;
            }
            if (e.NewValue != null)
            {
                e.NewValue.TemplateChanged += TemplateChangedHandler;
                e.NewValue.JobChanged += JobChangedHandler;
                e.NewValue.SpaceChanged += SpaceChangedHandler;
                e.NewValue.MoneyChanged += MoneyChangedHandler;
                e.NewValue.SatisfactionChanged += SatisfactionChangedHandler;
                e.NewValue.OwnedResourcesChanged += SourceModel_OwnedResourcesChanged;
                e.NewValue.OwnedResources.CollectionChanged += OwnedResourcesCollectionChangedHandler;
            }
        }

        private void SourceModel_OwnedResourcesChanged(object sender, ValueChangedEventArgs<ConsumeablesCollection> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.CollectionChanged -= OwnedResourcesCollectionChangedHandler;
            }
            if (e.NewValue != null)
            {
                e.NewValue.CollectionChanged += OwnedResourcesCollectionChangedHandler;
            }
        }
        #endregion
    }
}
