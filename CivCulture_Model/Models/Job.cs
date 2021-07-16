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
        #region Static Members
        public static Job Gatherer_Wilderness;
        public static Job Gatherer_Wheat;
        public static Job Farmer_Wheat;

        public static void InitializeJobs()
        {
            Resource.InitializeResources();
            Gatherer_Wilderness = new Job("Gatherer", 0, null, new List<Tuple<Consumeable, decimal>>() { }, new List<Tuple<Consumeable, decimal>>() { new Tuple<Consumeable, decimal>(Fundamental.Food, 1.2M) });

            Gatherer_Wheat = new Job("Gatherer", 0, null, new List<Tuple<Consumeable, decimal>>() { }, new List<Tuple<Consumeable, decimal>>() { new Tuple<Consumeable, decimal>(Resource.Wheat, 1M) });
            Farmer_Wheat = new Job("Wheat Farmer", 0, null, new List<Tuple<Consumeable, decimal>>() { }, new List<Tuple<Consumeable, decimal>>() { new Tuple<Consumeable, decimal>(Resource.Wheat, 2M) });
        }

        public static void InitializeTerrainResourceBindings()
        {
            Gatherer_Wilderness.Source = TerrainResource.Wilderness;

            Gatherer_Wheat.Source = TerrainResource.Wheat;
            Farmer_Wheat.Source = TerrainResource.Wheat;
        }
        #endregion

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

        public ConsumeablesCollection Inputs { get; protected set; }

        public ConsumeablesCollection Outputs { get; protected set; }

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
        public Job(string name, int priority, JobSource source, IEnumerable<Tuple<Consumeable, decimal>> inputs = null, IEnumerable<Tuple<Consumeable, decimal>> outputs = null)
        {
            Name = name;
            Priority = priority;
            Source = source;
            Inputs = new ConsumeablesCollection();
            if (inputs != null)
            {
                foreach (Tuple<Consumeable, decimal> pair in inputs)
                {
                    Inputs.Add(pair);
                }
            }
            Outputs = new ConsumeablesCollection();
            if (outputs != null)
            {
                foreach (Tuple<Consumeable, decimal> pair in outputs)
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
