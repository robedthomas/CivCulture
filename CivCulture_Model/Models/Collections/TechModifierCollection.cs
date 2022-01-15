using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Collections
{
    public class TechModifierCollection : Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, List<Modifier<decimal>>>
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
