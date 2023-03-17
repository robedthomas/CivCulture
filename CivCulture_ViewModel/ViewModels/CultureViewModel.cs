using CivCulture_Model.Events;
using CivCulture_Model.Models;
using CivCulture_ViewModel.Utilities;
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
        public event EventHandler WindowClosed;
        #endregion

        #region Fields
        private Culture sourceCulture;
        private Color cultureColor;
        private RelayCommand closeWindowRC;
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
                if (SourceCulture.Name != value)
                {
                    SourceCulture.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PopCount
        {
            get => SourceCulture.PopsOfCulture.Count;
        }

        public int TechCount
        {
            get => SourceCulture.ResearchedTechnologies.Count;
        }

        public TechTreeViewModel TechTreeVM { get; }

        public RelayCommand CloseWindowRC
        {
            get => closeWindowRC;
            set
            {
                if (closeWindowRC != value)
                {
                    closeWindowRC = value;
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
            TechTreeVM = new TechTreeViewModel(SourceCulture);
            CloseWindowRC = new RelayCommand((param) => OnWindowClosed());
        }
        #endregion

        #region Methods
        private void OnWindowClosed()
        {
            WindowClosed?.Invoke(this, new EventArgs());
        }

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
