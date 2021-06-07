using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Collections
{
    public enum NeedType
    {
        Necessity,
        Comfort,
        Luxury
    }

    public class NeedCollection : ObservableCollection<Tuple<NeedType, Consumeable, int>>
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
