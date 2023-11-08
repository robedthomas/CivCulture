using CivCulture_Model.Models.Modifiers;
using GenericUtilities.Observables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Collections
{
    public class TechModifierCollection : ObservableDictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<TechModifier<decimal>>>
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
        public override void Add(Tuple<StatModification, ComponentTemplate, Consumeable> key, ObservableCollection<TechModifier<decimal>> value)
        {
            if (ContainsKey(key))
            {
                foreach (TechModifier<decimal> mod in value)
                {
                    this[key].Add(mod);
                }
            }
            else
            {
                base.Add(key, value);
            }
        }

        public void Add(StatModification modType, ComponentTemplate templateType, Consumeable modifiedConsumeable, TechModifier<decimal> modification)
        {
            Tuple<StatModification, ComponentTemplate, Consumeable> targetTuple = new Tuple<StatModification, ComponentTemplate, Consumeable>(modType, templateType, modifiedConsumeable);
            if (!ContainsKey(targetTuple))
            {
                Add(targetTuple, new ObservableCollection<TechModifier<decimal>>());
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
                    this[key] = new ObservableCollection<TechModifier<decimal>>();
                }
                if (collectionToAdd[key] != null)
                {
                    foreach (TechModifier<decimal> mod in collectionToAdd[key])
                    {
                        this[key].Add(mod);
                    }
                }
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
                if (ContainsKey(key) && collectionToRemove[key] != null)
                {
                    foreach (TechModifier<decimal> mod in collectionToRemove[key])
                    {
                        this[key].Remove(mod);
                    }
                }
            }
        }
        #endregion
    }
}
