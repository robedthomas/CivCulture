using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Market : GameComponent
    {
        #region Delegates
        /// <summary>
        /// A delegate that defines how to calculate the price of a given consumeable based off of the supply and demand for that consumeable
        /// </summary>
        /// <param name="targetResource">The consumeable to calculate the price of</param>
        /// <param name="owner">The market in which the price is being calculated</param>
        /// <returns></returns>
        public delegate decimal CalculateResourcePriceDelegate(Consumeable targetResource, Market owner);
        #endregion

        #region Events
        public ValueChangedEventHandler<ConsumeablesCollection> DemandedResourcesChanged;
        public ValueChangedEventHandler<ConsumeablesCollection> SuppliedResourcesChanged;
        #endregion

        #region Fields
        private CalculateResourcePriceDelegate priceAlgorithm;
        private ConsumeablesCollection demandedResources;
        private ConsumeablesCollection suppliedResources;
        #endregion

        #region Properties
        public ConsumeablesCollection DemandedResources
        {
            get => demandedResources;
            set
            {
                if (demandedResources != value)
                {
                    ConsumeablesCollection oldValue = demandedResources;
                    demandedResources = value;
                    DemandedResourcesChanged?.Invoke(this, new ValueChangedEventArgs<ConsumeablesCollection>(oldValue, value));
                    if (demandedResources != null)
                    {
                        UpdateResourcePrices(demandedResources.Keys);
                    }
                }
            }
        }

        public ConsumeablesCollection SuppliedResources
        {
            get => suppliedResources;
            set
            {
                if (suppliedResources != value)
                {
                    ConsumeablesCollection oldValue = suppliedResources;
                    suppliedResources = value;
                    SuppliedResourcesChanged?.Invoke(this, new ValueChangedEventArgs<ConsumeablesCollection>(oldValue, value));
                    if (suppliedResources != null)
                    {
                        UpdateResourcePrices(suppliedResources.Keys);
                    }
                }
            }
        }

        public ConsumeablesCollection ResourcePrices
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        public Market(CalculateResourcePriceDelegate priceAlgorithm)
        {
            this.priceAlgorithm = priceAlgorithm;
            DemandedResources = new ConsumeablesCollection();
            SuppliedResources = new ConsumeablesCollection();
            ResourcePrices = new ConsumeablesCollection();
            foreach (Fundamental f in Fundamental.AllFundamentals)
            {
                ResourcePrices.Add(f, f.BaseValue);
            }
            foreach (Resource r in Resource.AllResources)
            {
                ResourcePrices.Add(r, r.BaseValue);
            }

            DemandedResources.CollectionChanged += DemandedResources_CollectionChanged;
            SuppliedResources.CollectionChanged += SuppliedResources_CollectionChanged;
        }
        #endregion

        #region Methods
        public void UpdateResourcePrices(IEnumerable<Consumeable> changedResources)
        {
            foreach (Consumeable changedResource in changedResources)
            {
                ResourcePrices[changedResource] = priceAlgorithm(changedResource, this);
            }
        }

        protected void OnResourcesPresentChanged(IEnumerable<Consumeable> newResources, IEnumerable<Consumeable> oldResources)
        {
            HashSet<Consumeable> changedResources = new HashSet<Consumeable>();
            if (newResources != null)
            {
                foreach (Consumeable newResource in newResources)
                {
                    changedResources.Add(newResource);
                }
            }
            if (oldResources != null)
            {
                foreach (Consumeable oldResource in oldResources)
                {
                    changedResources.Add(oldResource);
                }
            }
            UpdateResourcePrices(changedResources);
        }

        private void SuppliedResources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnResourcesPresentChanged(e.NewItems as IEnumerable<Consumeable>, e.OldItems as IEnumerable<Consumeable>);
        }

        private void DemandedResources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnResourcesPresentChanged(e.NewItems as IEnumerable<Consumeable>, e.OldItems as IEnumerable<Consumeable>);
        }
        #endregion
    }
}
