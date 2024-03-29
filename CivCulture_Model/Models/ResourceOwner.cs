﻿using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using GenericUtilities.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public abstract class ResourceOwner : GameComponent
    {
        #region Events
        public event ValueChangedEventHandler<ConsumeablesCollection> OwnedResourcesChanged;
        public event ValueChangedEventHandler<ConsumeablesCollection> ConsumedResourcesChanged;
        public event ValueChangedEventHandler<ConsumeablesCollection> ProducedResourcesChanged;
        public event ValueChangedEventHandler<decimal> MoneyChanged;
        #endregion

        #region Fields
        private ConsumeablesCollection ownedResources;
        private ConsumeablesCollection consumedResources;
        private ConsumeablesCollection producedResources;
        private decimal money;
        #endregion

        #region Properties
        public ConsumeablesCollection OwnedResources
        {
            get => ownedResources;
            protected set
            {
                if (ownedResources != value)
                {
                    ConsumeablesCollection oldResources = ownedResources;
                    ownedResources = value;
                    OwnedResourcesChanged?.Invoke(this, new ValueChangedEventArgs<ConsumeablesCollection>(oldResources, ownedResources));
                }
            }
        }

        public ConsumeablesCollection ConsumedResources
        {
            get => consumedResources;
            protected set
            {
                if (consumedResources != value)
                {
                    ConsumeablesCollection oldResources = consumedResources;
                    consumedResources = value;
                    ConsumedResourcesChanged?.Invoke(this, new ValueChangedEventArgs<ConsumeablesCollection>(oldResources, consumedResources));
                }
            }
        }

        public ConsumeablesCollection ProducedResources
        {
            get => producedResources;
            protected set
            {
                if (producedResources != value)
                {
                    ConsumeablesCollection oldResources = producedResources;
                    producedResources = value;
                    ProducedResourcesChanged?.Invoke(this, new ValueChangedEventArgs<ConsumeablesCollection>(oldResources, producedResources));
                }
            }
        }

        public decimal Money
        {
            get => money;
            set
            {
                if (money != value)
                {
                    decimal oldValue = money;
                    money = value;
                    MoneyChanged?.Invoke(this, new ValueChangedEventArgs<decimal>(oldValue, money));
                }
            }
        }
        #endregion

        #region Constructors
        public ResourceOwner()
        {
            OwnedResources = new ConsumeablesCollection();
            ConsumedResources = new ConsumeablesCollection();
            ProducedResources = new ConsumeablesCollection();
        }
        #endregion

        #region Methods
        public bool TrySell(Consumeable resourceToSell, decimal countToSell, decimal totalSaleValue, ResourceOwner buyer)
        {
            return TryResourceTransaction(this, buyer, resourceToSell, countToSell, totalSaleValue);
        }

        public static bool TryResourceTransaction(ResourceOwner seller, ResourceOwner buyer, Consumeable tradedResource, decimal countToTrade, decimal transactionMoneyValue)
        {
            if (countToTrade == 0)
            {
                return true;
            }
            else if (countToTrade > 0)
            {
                if (seller.OwnedResources.ContainsKey(tradedResource) && seller.OwnedResources[tradedResource] >= countToTrade)
                {
                    if (buyer.Money >= transactionMoneyValue)
                    {
                        seller.OwnedResources.Subtract(tradedResource, countToTrade);
                        buyer.OwnedResources.Add(tradedResource, countToTrade);
                        seller.Money += transactionMoneyValue;
                        buyer.Money -= transactionMoneyValue;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
