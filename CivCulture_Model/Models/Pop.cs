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
        #endregion

        #region Events
        public event ValueChangedEventHandler<Job> JobChanged;
        public event ValueChangedEventHandler<int> MoneyChanged;
        #endregion

        #region Properties
        public ObservableCollection<Resource> OwnedResources { get; protected set; } = new ObservableCollection<Resource>();
        public NeedCollection Needs { get; protected set; } = new NeedCollection();

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
        #endregion

        #region Methods
        public void WorkJob()
        {
            // @TODO
        }
        #endregion
    }
}
