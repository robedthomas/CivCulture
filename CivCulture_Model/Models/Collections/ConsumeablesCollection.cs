using GenericUtilities.Observables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Collections
{
    public class ConsumeablesCollection : ObservableDictionary<Consumeable, decimal>
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public decimal BaseValue
        {
            get
            {
                decimal value = 0M;
                foreach (KeyValuePair<Consumeable, decimal> pair in this)
                {
                    value += pair.Key.BaseValue * pair.Value;
                }
                return value;
            }
        }
        #endregion

        #region Constructors
        public ConsumeablesCollection() { }

        public ConsumeablesCollection(ConsumeablesCollection copyFrom) : base(copyFrom) { }
        #endregion

        #region Methods
        public new void Add(Consumeable consumeable, decimal count)
        {
            if (ContainsKey(consumeable))
            {
                this[consumeable] += count;
            }
            else
            {
                base.Add(consumeable, count);
            }
        }

        public void Add(ConsumeablesCollection collection)
        {
            foreach (KeyValuePair<Consumeable, decimal> pair in collection)
            {
                if (ContainsKey(pair.Key))
                {
                    this[pair.Key] += pair.Value;
                }
                else
                {
                    this[pair.Key] = pair.Value;
                }
            }
        }

        public void Add(NeedCollection needs)
        {
            foreach (NeedType needType in needs.Keys)
            {
                Add(needs[needType]);
            }
        }

        public void Subtract(ConsumeablesCollection collection)
        {
            foreach (KeyValuePair<Consumeable, decimal> pair in collection)
            {
                Subtract(pair.Key, pair.Value);
            }
        }

        public void Subtract(Consumeable consumeable, decimal countToSubtract)
        {
            if (ContainsKey(consumeable))
            {
                this[consumeable] -= countToSubtract;
            }
            else
            {
                this[consumeable] = -countToSubtract;
            }
        }

        public bool IsSatisfiedBy(ConsumeablesCollection consumeables)
        {
            foreach (KeyValuePair<Consumeable, decimal> requirement in this)
            {
                if (!consumeables.ContainsKey(requirement.Key) || consumeables[requirement.Key] < requirement.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearNonAccumulatingConsumeables()
        {
            foreach (Consumeable resource in Keys)
            {
                if (!resource.Accumulates)
                {
                    this[resource] = 0;
                }
            }
        }

        public decimal GetMarketValue(Market market)
        {
            decimal totalValue = 0;
            foreach (KeyValuePair<Consumeable, decimal> pair in this)
            {
                totalValue += market.ResourcePrices[pair.Key] * pair.Value;
            }
            return totalValue;
        }

        public static ConsumeablesCollection Sum(IEnumerable<ConsumeablesCollection> collections)
        {
            ConsumeablesCollection output = new ConsumeablesCollection();
            foreach (ConsumeablesCollection collection in collections)
            {
                output.Add(collection);
            }
            return output;
        }
        #endregion
    }
}
