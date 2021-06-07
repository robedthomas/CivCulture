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
        private Pop worker;
        #endregion

        #region Events
        public event ValueChangedEventHandler<Pop> WorkerChanged;
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public int Priority { get; protected set; }

        public JobSource Source { get; protected set; }

        public FundamentalCollection Inputs { get; protected set; }

        public FundamentalCollection Outputs { get; protected set; }

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
        #endregion

        #region Constructors
        public Job(string name, int priority, JobSource source, IEnumerable<Tuple<Fundamental, int>> inputs = null, IEnumerable<Tuple<Fundamental, int>> outputs = null)
        {
            Name = name;
            Priority = priority;
            Source = source;
            Inputs = new FundamentalCollection();
            if (inputs != null)
            {
                foreach (Tuple<Fundamental, int> pair in inputs)
                {
                    Inputs.Add(pair);
                }
            }
            Outputs = new FundamentalCollection();
            if (outputs != null)
            {
                foreach (Tuple<Fundamental, int> pair in outputs)
                {
                    Outputs.Add(pair);
                }
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
