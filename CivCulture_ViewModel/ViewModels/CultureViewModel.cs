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
                    if (sourceCulture != null)
                    {
                        UnsubscribeFromModelEvents(sourceCulture);
                    }
                    sourceCulture = value;
                    if (sourceCulture != null)
                    {
                        SubscribeToModelEvents(sourceCulture);
                    }
                    OnPropertyChanged(nameof(Name));
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

        public string Name
        {
            get => SourceCulture.Name;
            set
            {
                SourceCulture.Name = value;
                OnPropertyChanged();
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
        private void UnsubscribeFromModelEvents(Culture model)
        {
            model.NameChanged -= Culture_NameChanged;
        }

        private void SubscribeToModelEvents(Culture model)
        {
            model.NameChanged += Culture_NameChanged;
        }

        private void Culture_NameChanged(object sender, ValueChangedEventArgs<string> e)
        {
            OnPropertyChanged(nameof(Name));
        }
        #endregion
    }
}
