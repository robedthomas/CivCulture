using CivCulture_Model.Events;
using CivCulture_Model.Models;
using CivCulture_ViewModel.Utilities;
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
        private GameInstance sourceInstance;
        private GameMapViewModel mapVM;
        private MapSpaceDetailsViewModel selectedSpaceDetails;
        private RelayCommand endTurnRC;
        #endregion

        #region Events
        #endregion

        #region Properties
        public GameInstance SourceInstance
        {
            get => sourceInstance;
            set
            {
                if (sourceInstance != value)
                {
                    if (sourceInstance != null)
                    {
                        sourceInstance.MapChanged -= SourceInstance_MapChanged;
                    }
                    sourceInstance = value;
                    if (sourceInstance != null)
                    {
                        sourceInstance.MapChanged += SourceInstance_MapChanged;
                    }
                    if (sourceInstance.Map != null)
                    {
                        SourceInstance_MapChanged(this, null);
                    }
                    OnPropertyChanged();
                }
            }
        }

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

        public RelayCommand EndTurnRC
        {
            get => endTurnRC;
            set
            {
                if (endTurnRC != value)
                {
                    endTurnRC = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public GameInstanceViewModel(GameInstance sourceInstance)
        {
            SourceInstance = sourceInstance;
            EndTurnRC = new RelayCommand(EndTurn, CanEndTurn);
        }
        #endregion

        #region Methods
        private void MapVM_SelectedSpaceChanged(object sender, ValueChangedEventArgs<MapSpaceViewModel> e)
        {
            if (e.NewValue is null)
            {
                SelectedSpaceDetails = null;
            }
            else
            {
                SelectedSpaceDetails = new MapSpaceDetailsViewModel(e.NewValue.SourceSpace);
            }
        }

        private void SourceInstance_MapChanged(object sender, ValueChangedEventArgs<GameMap> e)
        {
            MapVM = new GameMapViewModel(SourceInstance.Map);
        }

        private void EndTurn(object param)
        {
            SourceInstance.PassTurn();
        }

        private bool CanEndTurn(object param)
        {
            return true; // @TODO
        }
        #endregion
    }
}
