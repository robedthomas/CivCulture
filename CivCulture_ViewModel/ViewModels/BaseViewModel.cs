using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Fields
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
