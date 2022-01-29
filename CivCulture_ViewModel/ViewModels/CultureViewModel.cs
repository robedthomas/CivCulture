using CivCulture_Model.Events;
using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CivCulture_ViewModel.ViewModels
{
    public class CultureViewModel : BaseViewModel
    {
        #region Events
        public event ValueChangedEventHandler<Color> CultureColorChanged;
        #endregion

        #region Fields
        private Culture sourceCulture;
        private Color cultureColor;
        #endregion

        #region Properties
        public Culture SourceCulture
        {
            get => sourceCulture;
            set
            {
                if (sourceCulture != value)
                {
                    sourceCulture = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color CultureColor
        {
            get => cultureColor;
            set
            {
                if (cultureColor != value)
                {
                    Color oldValue = cultureColor;
                    cultureColor = value;
                    CultureColorChanged?.Invoke(this, new ValueChangedEventArgs<Color>(oldValue, value));
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public CultureViewModel(Culture sourceCulture, Color cultureColor)
        {
            SourceCulture = sourceCulture;
            CultureColor = cultureColor;
        }
        #endregion

        #region Methods
        #endregion
    }
}
