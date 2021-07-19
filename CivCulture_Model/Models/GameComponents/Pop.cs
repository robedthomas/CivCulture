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
    public class Pop : GameComponent 
    {
        #region Fields
        private Job job;
        private MapSpace space;
        private decimal money;
        private ConsumeablesCollection ownedResources;
        private NeedCollection needs;
        #endregion

        #region Events
        public event ValueChangedEventHandler<Job> JobChanged;
        public event ValueChangedEventHandler<MapSpace> SpaceChanged;
        public event ValueChangedEventHandler<decimal> MoneyChanged;
        public event ValueChangedEventHandler<ConsumeablesCollection> OwnedResourcesChanged;
        public event ValueChangedEventHandler<NeedCollection> NeedsChanged;
        #endregion

        #region Properties
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
        public NeedCollection Needs
        {
            get => needs;
            protected set
            {
                if (needs != value)
                {
                    NeedCollection oldNeeds = needs;
                    needs = value;
                    NeedsChanged?.Invoke(this, new ValueChangedEventArgs<NeedCollection>(oldNeeds, needs));
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
        public Pop()
        {
            OwnedResources = new ConsumeablesCollection();
            Needs = new NeedCollection();
        }
        #endregion

        #region Methods
        public void WorkJob()
        {
            // @TODO
        }
        #endregion
    }
}
