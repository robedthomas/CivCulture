using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Forecasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Template = template;
            Culture = culture;
            JobChanged += This_JobChanged;
            SpaceChanged += This_SpaceChanged;
            Satisfaction = 1M;
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
