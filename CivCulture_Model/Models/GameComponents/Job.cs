using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Job : GameComponent
    {
        #region Fields
        private JobTemplate template;
        private Pop worker;
        private MapSpace space;
        #endregion

        #region Events
        public event ValueChangedEventHandler<Pop> WorkerChanged;
        public event ValueChangedEventHandler<MapSpace> SpaceChanged;
        public event ValueChangedEventHandler<JobTemplate> TemplateChanged;
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
        #endregion

        #region Constructors
        public Job(JobTemplate template)
        {
            Template = template;
            WorkerChanged += This_WorkerChanged;
            SpaceChanged += This_SpaceChanged;
        }

        private void This_WorkerChanged(object sender, ValueChangedEventArgs<Pop> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.Job = null;
            }
            if (e.NewValue != null)
            {
                e.NewValue.Job = this;
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
