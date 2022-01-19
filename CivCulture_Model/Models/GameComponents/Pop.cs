using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Forecasts;
using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("{Template.Name} Pop")]
    public class Pop : ResourceOwner, ITemplated<PopTemplate>
    {
        #region Fields
        private PopTemplate template;
        private Job job;
        private MapSpace space;
        private decimal satisfaction;
        private Culture culture;
        #endregion

        #region Events
        public event ValueChangedEventHandler<PopTemplate> TemplateChanged;
        public event ValueChangedEventHandler<Job> JobChanged;
        public event ValueChangedEventHandler<MapSpace> SpaceChanged;
        public event ValueChangedEventHandler<decimal> SatisfactionChanged;
        public event ValueChangedEventHandler<Culture> CultureChanged;
        #endregion

        #region Properties
        public PopForecast Forecast { get; private set; }

        public TechModifierCollection TechModifiers { get; private set; }

        private Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler> ModifiersListHandlers { get; set; }

        public PopTemplate Template
        {
            get => template;
            set
            {
                if (template != value)
                {
                    PopTemplate oldValue = template;
                    template = value;
                    TemplateChanged?.Invoke(this, new ValueChangedEventArgs<PopTemplate>(oldValue, template));
                }
            }
        }

        public Job Job
        {
            get => job;
            set
            {
                if (job != value)
                {
                    Job oldValue = job;
                    job = value;
                    JobChanged?.Invoke(this, new ValueChangedEventArgs<Job>(oldValue, job));
                }
            }
        }

        public MapSpace Space
        {
            get => space;
            set
            {
                if (space != value)
                {
                    MapSpace oldValue = space;
                    space = value;
                    SpaceChanged?.Invoke(this, new ValueChangedEventArgs<MapSpace>(oldValue, space));
                }
            }
        }

        public NeedCollection Needs { get; protected set; }

        public decimal Satisfaction
        {
            get => satisfaction;
            set
            {
                if (satisfaction != value)
                {
                    if (value > 1)
                    {
                        value = 1;
                    }
                    if (value < 0)
                    {
                        value = 0;
                    }
                    decimal oldValue = satisfaction;
                    satisfaction = value;
                    SatisfactionChanged?.Invoke(this, new ValueChangedEventArgs<decimal>(oldValue, satisfaction));
                }
            }
        }

        public Culture Culture
        {
            get => culture;
            set
            {
                if (culture != value)
                {
                    Culture oldValue = culture;
                    culture = value;
                    CultureChanged?.Invoke(this, new ValueChangedEventArgs<Culture>(oldValue, value));
                }
            }
        }
        #endregion

        #region Constructors
        public Pop(PopTemplate template, Culture culture) : base()
        {
            Forecast = new PopForecast(this);
            TechModifiers = new TechModifierCollection();
            ModifiersListHandlers = new Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler>();
            Template = template;
            Needs = new NeedCollection(Template.Needs);
            Culture = culture;
            Satisfaction = 1M;

            TemplateChanged += This_TemplateChanged;
            JobChanged += This_JobChanged;
            SpaceChanged += This_SpaceChanged;
            CultureChanged += This_CultureChanged;
        }
        #endregion

        #region Methods
        private void ApplyModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, Modifier<decimal> modifier)
        {
            NeedType? targetNeedType = null;
            if (modifierKey.Item1 == StatModification.PopNecessities)
            {
                targetNeedType = NeedType.Necessity;
            }
            else if (modifierKey.Item1 == StatModification.PopComforts)
            {
                targetNeedType = NeedType.Comfort;
            }
            else if (modifierKey.Item1 == StatModification.PopLuxuries)
            {
                targetNeedType = NeedType.Luxury;
            }
            if (targetNeedType != null)
            {
                if (!Needs[(NeedType)targetNeedType].ContainsKey(modifierKey.Item3))
                {
                    Needs[(NeedType)targetNeedType].Add(modifierKey.Item3, 0);
                }
                Needs[(NeedType)targetNeedType][modifierKey.Item3] += modifier.Modification;
            }
            // @TODO: handle other types of StatModification
        }

        private void UnapplyModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, Modifier<decimal> modifier)
        {
            NeedType? targetNeedType = null;
            if (modifierKey.Item1 == StatModification.PopNecessities)
            {
                targetNeedType = NeedType.Necessity;
            }
            else if (modifierKey.Item1 == StatModification.PopComforts)
            {
                targetNeedType = NeedType.Comfort;
            }
            else if (modifierKey.Item1 == StatModification.PopLuxuries)
            {
                targetNeedType = NeedType.Luxury;
            }
            if (targetNeedType != null)
            {
                Needs[(NeedType)targetNeedType][modifierKey.Item3] -= modifier.Modification;
            }
            // @TODO: handle other types of StatModification
        }

        private NotifyCollectionChangedEventHandler GetTechModifierListChangedHandler(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey)
        {
            return new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Modifier<decimal> newMod in e.NewItems)
                    {
                        ApplyModifier(modifierKey, newMod);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (Modifier<decimal> oldMod in e.OldItems)
                    {
                        UnapplyModifier(modifierKey, oldMod);
                    }
                }
            });
        }

        private bool TryAddTechModifierList(KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifier)
        {
            NotifyCollectionChangedEventHandler newHandler = GetTechModifierListChangedHandler(modifier.Key);
            if (modifier.Key.Item2 == PopTemplate.ALL || modifier.Key.Item2 == Template)
            {
                TechModifiers.Add(modifier.Key, modifier.Value);
                TechModifiers[modifier.Key].CollectionChanged += newHandler;
                ModifiersListHandlers.Add(modifier.Key, newHandler);
                foreach (Modifier<decimal> mod in modifier.Value)
                {
                    ApplyModifier(modifier.Key, mod);
                }
                return true;
            }
            return false;
        }

        private bool TryRemoveTechModifierList(KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifier)
        {
            if (TechModifiers.Contains(modifier))
            {
                foreach (Modifier<decimal> mod in modifier.Value)
                {
                    UnapplyModifier(modifier.Key, mod);
                }
                TechModifiers[modifier.Key].CollectionChanged -= ModifiersListHandlers[modifier.Key];
                TechModifiers.Remove(modifier.Key);
                ModifiersListHandlers.Remove(modifier.Key);
                return true;
            }
            return false;
        }

        private void This_CultureChanged(object sender, ValueChangedEventArgs<Culture> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.TechModifiers.CollectionChanged -= Culture_TechModifiers_CollectionChanged;
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifierPair in e.OldValue.TechModifiers)
                {
                    TryRemoveTechModifierList(modifierPair);
                }
            }
            if (e.NewValue != null)
            {
                e.NewValue.TechModifiers.CollectionChanged += Culture_TechModifiers_CollectionChanged; ;
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifierPair in e.NewValue.TechModifiers)
                {
                    TryAddTechModifierList(modifierPair);
                }
            }
        }

        private void Culture_TechModifiers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> newPair in e.NewItems)
                {
                    TryAddTechModifierList(newPair);
                }
            }
            if (e.OldItems != null)
            {
                foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> oldPair in e.OldItems)
                {
                    TryRemoveTechModifierList(oldPair);
                }
            }
        }

        private void This_TemplateChanged(object sender, ValueChangedEventArgs<PopTemplate> e)
        {
            // Remove all tech modifiers tied to the previous template
            IEnumerable<KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>>> modifierListsToRemove = TechModifiers.Where(pair => pair.Key.Item2 == PopTemplate.ALL || pair.Key.Item2 == e.OldValue);
            foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifierPair in modifierListsToRemove)
            {
                TryRemoveTechModifierList(modifierPair);
            }
            Needs.Clear();
            if (e.NewValue != null)
            {
                foreach (NeedType need in e.NewValue.Needs.Keys)
                {
                    Needs.Add(need, new ConsumeablesCollection(e.NewValue.Needs[need]));
                }
            }
            // Add all tech modifiers tied to the new template
            IEnumerable<KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>>> modifierListsToAdd = Culture.TechModifiers.Where(pair => pair.Key.Item2 == PopTemplate.ALL || pair.Key.Item2 == e.NewValue);
            foreach (KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifierPair in modifierListsToAdd)
            {
                TryAddTechModifierList(modifierPair);
            }
        }

        private void This_JobChanged(object sender, ValueChangedEventArgs<Job> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.Worker = null;
            }
            if (e.NewValue != null)
            {
                e.NewValue.Worker = this;
            }
        }

        private void This_SpaceChanged(object sender, ValueChangedEventArgs<MapSpace> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.Pops.Remove(this);
            }
            if (e.NewValue != null)
            {
                e.NewValue.Pops.Add(this);
            }
        }
        #endregion
    }
}
