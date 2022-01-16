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
    public class TechModifierCollection : ObservableDictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>>
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
        public override void Add(Tuple<StatModification, ComponentTemplate, Consumeable> key, ObservableCollection<Modifier<decimal>> value)
        {
            if (ContainsKey(key))
            {
                foreach (Modifier<decimal> mod in value)
                {
                    this[key].Add(mod);
                }
            }
            else
            {
                base.Add(key, new ObservableCollection<Modifier<decimal>>(value));
            }
        }

        public void Add(StatModification modType, ComponentTemplate templateType, Consumeable modifiedConsumeable, Modifier<decimal> modification)
        {
            Tuple<StatModification, ComponentTemplate, Consumeable> targetTuple = new Tuple<StatModification, ComponentTemplate, Consumeable>(modType, templateType, modifiedConsumeable);
            if (!ContainsKey(targetTuple))
            {
                Add(targetTuple, new ObservableCollection<Modifier<decimal>>());
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
                    this[key] = new ObservableCollection<Modifier<decimal>>();
                }
                foreach (Modifier<decimal> mod in collectionToAdd[key])
                {
                    this[key].Add(mod);
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
