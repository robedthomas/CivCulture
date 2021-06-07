using CivCulture_Model.Events;
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
        #endregion

        #region Events
        public event ValueChangedEventHandler<Job> JobChanged;
        #endregion

        #region Properties
        public ObservableCollection<Resource> OwnedResources { get; protected set; } = new ObservableCollection<Resource>();

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
