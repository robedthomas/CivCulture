﻿using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Pop : GameComponent 
    {
        #region Fields
        private PopTemplate template;
        private Job job;
        private MapSpace space;
        private decimal money;
        private ConsumeablesCollection ownedResources;
        #endregion

        #region Events
        public event ValueChangedEventHandler<PopTemplate> TemplateChanged;
        public event ValueChangedEventHandler<Job> JobChanged;
        public event ValueChangedEventHandler<MapSpace> SpaceChanged;
        public event ValueChangedEventHandler<decimal> MoneyChanged;
        public event ValueChangedEventHandler<ConsumeablesCollection> OwnedResourcesChanged;
        #endregion

        #region Properties
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
        public Pop(PopTemplate template)
        {
            Template = template;
            OwnedResources = new ConsumeablesCollection();
            JobChanged += This_JobChanged;
            SpaceChanged += This_SpaceChanged;
        }
        #endregion

        #region Methods
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
