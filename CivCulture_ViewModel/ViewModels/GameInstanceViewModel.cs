using CivCulture_Model.Events;
using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class GameInstanceViewModel : BaseViewModel
    {
        #region Fields
        private GameMapViewModel mapVM;
        private MapSpaceDetailsViewModel selectedSpaceDetails;
        #endregion

        #region Events
        #endregion

        #region Properties
        public GameInstance SourceInstance { get; set; }

        public GameMapViewModel MapVM
        {
            get => mapVM;
            set
            {
                if (mapVM != value)
                {
                    if (mapVM != null)
                    {
                        mapVM.SelectedSpaceChanged -= MapVM_SelectedSpaceChanged;
                    }
                    mapVM = value;
                    if (mapVM != null)
                    {
                        mapVM.SelectedSpaceChanged += MapVM_SelectedSpaceChanged;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public MapSpaceDetailsViewModel SelectedSpaceDetails
        {
            get => selectedSpaceDetails;
            set
            {
                if (selectedSpaceDetails != value)
                {
                    selectedSpaceDetails = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public GameInstanceViewModel(GameInstance sourceInstance)
        {
            SourceInstance = sourceInstance;
        }
        #endregion

        #region Methods
        public void GenerateNewMap()
        {
            SourceInstance.GenerateMap();
            MapVM = new GameMapViewModel(SourceInstance.Map);
        }

        private void MapVM_SelectedSpaceChanged(object sender, ValueChangedEventArgs<MapSpaceViewModel> e)
        {
            SelectedSpaceDetails = new MapSpaceDetailsViewModel(e.NewValue.SourceSpace);
        }
        #endregion
    }
}
