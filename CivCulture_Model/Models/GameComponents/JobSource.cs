using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public abstract class JobSource : GameComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public ObservableCollection<Job> ChildJobs { get; protected set; } = new ObservableCollection<Job>();
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
