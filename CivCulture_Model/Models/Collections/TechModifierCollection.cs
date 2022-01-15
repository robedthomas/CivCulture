using CivCulture_Model.Models.Modifiers;
using GenericUtilities.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Collections
{
    public class TechModifierCollection : ObservableDictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, List<Modifier<decimal>>>
    {
        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public void Add(StatModification modType, ComponentTemplate templateType, Consumeable modifiedConsumeable, Modifier<decimal> modification)
        {
            Tuple<StatModification, ComponentTemplate, Consumeable> targetTuple = new Tuple<StatModification, ComponentTemplate, Consumeable>(modType, templateType, modifiedConsumeable);
            if (!ContainsKey(targetTuple))
            {
                Add(targetTuple, new List<Modifier<decimal>>());
            }
            this[targetTuple].Add(modification);
        }

        public void AddRange(TechModifierCollection collectionToAdd)
        {
            if (collectionToAdd is null)
            {
                throw new ArgumentNullException();
            }
            foreach (Tuple<StatModification, ComponentTemplate, Consumeable> key in collectionToAdd.Keys)
            {
                if (!ContainsKey(key))
                {
                    this[key] = new List<Modifier<decimal>>();
                }
                this[key].AddRange(collectionToAdd[key]);
            }
        }

        public void RemoveRange(TechModifierCollection collectionToRemove)
        {
            if (collectionToRemove is null)
            {
                throw new ArgumentNullException();
            }
            foreach (Tuple<StatModification, ComponentTemplate, Consumeable> key in collectionToRemove.Keys)
            {
                if (ContainsKey(key))
                {
                    foreach (Modifier<decimal> mod in collectionToRemove[key])
                    {
                        this[key].Remove(mod);
                    }
                }
            }
        }
        #endregion
    }
}
