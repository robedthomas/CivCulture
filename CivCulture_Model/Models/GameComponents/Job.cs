using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using GenericUtilities.Observables;
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
    [DebuggerDisplay("{Template.Name} Job")]
    public class Job : GameComponent
    {
        #region Fields
        private JobTemplate template;
        private JobSource source;
        private Pop worker;
        private MapSpace space;
        #endregion

        #region Events
        public event ValueChangedEventHandler<Pop> WorkerChanged;
        public event ValueChangedEventHandler<MapSpace> SpaceChanged;
        public event ValueChangedEventHandler<JobTemplate> TemplateChanged;
        public event ValueChangedEventHandler<JobSource> SourceChanged;
        #endregion

        #region Properties
        public JobTemplate Template
        {
            get => template;
            set
            {
                if (template != value)
                {
                    JobTemplate oldValue = template;
                    template = value;
                    TemplateChanged?.Invoke(this, new ValueChangedEventArgs<JobTemplate>(oldValue, template));
                }
            }
        }

        public JobSource Source
        {
            get => source;
            set
            {
                if (source != value)
                {
                    JobSource oldValue = source;
                    source = value;
                    SourceChanged?.Invoke(this, new ValueChangedEventArgs<JobSource>(oldValue, source));
                }
            }
        }

        public Pop Worker
        {
            get => worker;
            set
            {
                if (worker != value)
                {
                    Pop oldValue = worker;
                    worker = value;
                    WorkerChanged?.Invoke(this, new ValueChangedEventArgs<Pop>(oldValue, worker));
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

        public ConsumeablesCollection Inputs { get; protected set; }

        public ConsumeablesCollection Outputs { get; protected set; }

        public TechModifierCollection InputTechModifiers { get; protected set; }

        public TechModifierCollection OutputTechModifiers { get; protected set; }

        private Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler> ModifiersListHandlers { get; set; }
        #endregion

        #region Constructors
        public Job(JobTemplate template, JobSource source)
        {
            Template = template;
            Source = source;
            Inputs = new ConsumeablesCollection(Template.Inputs);
            Outputs = new ConsumeablesCollection(Template.Outputs);
            InputTechModifiers = new TechModifierCollection();
            OutputTechModifiers = new TechModifierCollection();
            ModifiersListHandlers = new Dictionary<Tuple<StatModification, ComponentTemplate, Consumeable>, NotifyCollectionChangedEventHandler>();
            WorkerChanged += This_WorkerChanged;
            SpaceChanged += This_SpaceChanged;
        }

        private void AddTechModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, Modifier<decimal> modifier, ConsumeablesCollection modifiedCollection)
        {
            if (!modifiedCollection.ContainsKey(modifierKey.Item3))
            {
                modifiedCollection[modifierKey.Item3] = modifier.Modification;
            }
            else
            {
                modifiedCollection[modifierKey.Item3] += modifier.Modification;
            }
        }

        private void RemoveTechModifier(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey, Modifier<decimal> modifier, ConsumeablesCollection modifiedCollection)
        {
            modifiedCollection[modifierKey.Item3] -= modifier.Modification;
        }

        private NotifyCollectionChangedEventHandler GetTechModifierListChangedHandler(Tuple<StatModification, ComponentTemplate, Consumeable> modifierKey)
        {
            return new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Modifier<decimal> newMod in e.NewItems)
                    {
                        if (modifierKey.Item1 == StatModification.JobInputs)
                        {
                            AddTechModifier(modifierKey, newMod, Inputs);
                        }
                        else if (modifierKey.Item1 == StatModification.JobOutputs)
                        {
                            AddTechModifier(modifierKey, newMod, Outputs);
                        }
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (Modifier<decimal> oldMod in e.OldItems)
                    {
                        if (modifierKey.Item1 == StatModification.JobInputs)
                        {
                            RemoveTechModifier(modifierKey, oldMod, Inputs);
                        }
                        else if (modifierKey.Item1 == StatModification.JobOutputs)
                        {
                            RemoveTechModifier(modifierKey, oldMod, Outputs);
                        }
                    }
                }
            });
        }

        private bool TryAddTechModifierList(KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifier)
        {
            bool success = false;
            ConsumeablesCollection targetCollection = null;
            NotifyCollectionChangedEventHandler newHandler = GetTechModifierListChangedHandler(modifier.Key);
            if (modifier.Key.Item2 == null || modifier.Key.Item2 == Template)
            {
                if (modifier.Key.Item1 == StatModification.JobInputs)
                {
                    InputTechModifiers.Add(modifier.Key, modifier.Value);
                    targetCollection = Inputs;
                    success = true;
                }
                else if (modifier.Key.Item1 == StatModification.JobOutputs)
                {
                    OutputTechModifiers.Add(modifier.Key, modifier.Value);
                    targetCollection = Outputs;
                    success = true;
                }
            }
            if (success)
            {
                modifier.Value.CollectionChanged += newHandler;
                ModifiersListHandlers.Add(modifier.Key, newHandler);
                foreach (Modifier<decimal> newMod in modifier.Value)
                {
                    AddTechModifier(modifier.Key, newMod, targetCollection);
                }
            }
            return success;
        }

        private bool TryRemoveTechModifierList(KeyValuePair<Tuple<StatModification, ComponentTemplate, Consumeable>, ObservableCollection<Modifier<decimal>>> modifier)
        {
            bool success = false;
            ConsumeablesCollection targetCollection = null;
            if (InputTechModifiers.Contains(modifier))
            {
                InputTechModifiers.Remove(modifier.Key);
                targetCollection = Inputs;
                success = true;
            }
            else if (OutputTechModifiers.Contains(modifier))
            {
                OutputTechModifiers.Remove(modifier.Key);
                targetCollection = Outputs;
                success = true;
            }
            if (success)
            {
                modifier.Value.CollectionChanged -= ModifiersListHandlers[modifier.Key];
                ModifiersListHandlers.Remove(modifier.Key);
                foreach (Modifier<decimal> removedMod in modifier.Value)
                {
                    RemoveTechModifier(modifier.Key, removedMod, targetCollection);
                }
            }
            return success;
        }

        private void This_WorkerChanged(object sender, ValueChangedEventArgs<Pop> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.Job = null;
                e.OldValue.CultureChanged -= Worker_CultureChanged;
            }
            if (e.NewValue != null)
            {
                e.NewValue.Job = this;
                e.NewValue.CultureChanged += Worker_CultureChanged;
                if (e.OldValue != null)
                {
                    Worker_CultureChanged(e.NewValue, new ValueChangedEventArgs<Culture>(e.OldValue.Culture, e.NewValue.Culture));
                }
                else
                {
                    Worker_CultureChanged(e.NewValue, new ValueChangedEventArgs<Culture>(null, e.NewValue.Culture));
                }
            }
        }

        private void Worker_CultureChanged(object sender, ValueChangedEventArgs<Culture> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.TechModifiers.CollectionChanged -= Culture_TechModifiers_CollectionChanged;
            }
            InputTechModifiers.Clear();
            OutputTechModifiers.Clear();
            if (e.NewValue != null)
            {
                e.NewValue.TechModifiers.CollectionChanged += Culture_TechModifiers_CollectionChanged;
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

        private void This_SpaceChanged(object sender, ValueChangedEventArgs<MapSpace> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.Jobs.Remove(this);
            }
            if (e.NewValue != null)
            {
                e.NewValue.Jobs.Add(this);
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
