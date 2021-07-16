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
        private int money;
        private ObservableCollection<Tuple<Resource, int>> ownedResources;
        private NeedCollection needs;
        #endregion

        #region Events
        public event ValueChangedEventHandler<Job> JobChanged;
        public event ValueChangedEventHandler<int> MoneyChanged;
        public event ValueChangedEventHandler<ObservableCollection<Tuple<Resource, int>>> OwnedResourcesChanged;
        public event ValueChangedEventHandler<NeedCollection> NeedsChanged;
        #endregion

        #region Properties
        public ObservableCollection<Tuple<Resource, int>> OwnedResources
        {
            get => ownedResources;
            protected set
            {
                if (ownedResources != value)
                {
                    ObservableCollection<Tuple<Resource, int>> oldResources = ownedResources;
                    ownedResources = value;
                    OwnedResourcesChanged?.Invoke(this, new ValueChangedEventArgs<ObservableCollection<Tuple<Resource, int>>>(oldResources, ownedResources));
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

        public int Money
        {
            get => money;
            set
            {
                if (money != value)
                {
                    int oldValue = money;
                    money = value;
                    MoneyChanged?.Invoke(this, new ValueChangedEventArgs<int>(oldValue, money));
                }
            }
        }
        #endregion

        #region Constructors
        public Pop()
        {
            OwnedResources = new ObservableCollection<Tuple<Resource, int>>();
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
