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
        #endregion

        #region Methods
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

        public void Subtract(ConsumeablesCollection collection)
        {
            foreach (KeyValuePair<Consumeable, decimal> pair in collection)
            {
                if (ContainsKey(pair.Key))
                {
                    this[pair.Key] -= pair.Value;
                }
                else
                {
                    this[pair.Key] = -pair.Value;
                }
            }
        }

        public bool IsSatisfiedBy(ConsumeablesCollection consumeables)
        {
            foreach (KeyValuePair<Consumeable, decimal> requirement in this)
            {
                if (!consumeables.ContainsKey(requirement.Key) || consumeables[requirement.Key] < requirement.Value )
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
