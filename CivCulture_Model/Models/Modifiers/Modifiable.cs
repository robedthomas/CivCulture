using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Modifiers
{
    public enum StatModification
    {
        PopNecessities,
        PopComforts,
        PopLuxuries,
        PopProgressFromSatisfaction,
        JobInputs,
        JobOutputs,
        SpaceProductionThroughput,
        BuildingUnlocked,
    }

    public abstract class Modifiable<TValue>
    {
        #region Events
        public event ValueChangedEventHandler<TValue> ValueChanged;
        #endregion

        #region Fields
        private TValue value;
        private ObservableCollection<Modifier<TValue>> modifiers;
        private ObservableCollection<Modifier<decimal>> factorModifiers;
        #endregion

        #region Properties
        public TValue Value
        {
            get => value;
            protected set
            {
                if ((this.value == null && value != null) || !this.value.Equals(value))
                {
                    TValue oldValue = this.value;
                    this.value = value;
                    ValueChanged?.Invoke(this, new ValueChangedEventArgs<TValue>(oldValue, value));
                }
            }
        }

        public ObservableCollection<Modifier<TValue>> Modifiers
        {
            get => modifiers;
            protected set
            {
                if (modifiers != value)
                {
                    if (modifiers != null)
                    {
                        modifiers.CollectionChanged -= Modifiers_CollectionChanged;
                    }
                    modifiers = value;
                    if (modifiers != null)
                    {
                        modifiers.CollectionChanged += Modifiers_CollectionChanged;
                    }
                }
            }
        }

        public ObservableCollection<Modifier<decimal>> FactorModifiers
        {
            get => factorModifiers;
            protected set
            {
                if (factorModifiers != value)
                {
                    if (factorModifiers != null)
                    {
                        factorModifiers.CollectionChanged += FactorModifiers_CollectionChanged;
                    }
                    factorModifiers = value;
                    if (factorModifiers != null)
                    {
                        factorModifiers.CollectionChanged += FactorModifiers_CollectionChanged;
                    }
                }
            }
        }
        #endregion

        #region Constructors
        public Modifiable()
        {
            Modifiers = new ObservableCollection<Modifier<TValue>>();
            FactorModifiers = new ObservableCollection<Modifier<decimal>>();
        }
        #endregion

        #region Methods
        public abstract TValue AggregateModifiers(IEnumerable<Modifier<TValue>> modifiers, IEnumerable<Modifier<decimal>> factorModifiers);

        private void Modifiers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null || e.OldItems != null)
            {
                Value = AggregateModifiers(Modifiers, FactorModifiers);
            }
        }

        private void FactorModifiers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null || e.OldItems != null)
            {
                Value = AggregateModifiers(Modifiers, FactorModifiers);
            }
        }
        #endregion
    }
}
