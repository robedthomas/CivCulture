using CivCulture_Model.Events;
using CivCulture_Model.Models;
using CivCulture_ViewModel.Utilities;
using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class GameMapViewModel : BaseViewModel
    {
        #region Fields
        public const MapMode DEFAULT_MAP_MODE = MapMode.Culture;

        private GameMap sourceMap;
        private int numRows, numColumns;
        private ObservableCollection<MapSpaceViewModel> spaceVms;
        private MapSpaceViewModel selectedSpace;
        private MapSpaceDetailsViewModel selectedSpaceDetails;
        private MapMode selectedMapMode;
        #endregion

        #region Events
        public ValueChangedEventHandler<MapSpaceViewModel> SelectedSpaceChanged;
        public ValueChangedEventHandler<MapMode> SelectedMapModeChanged;
        #endregion

        #region Properties
        public GameMap SourceMap 
        {
            get => sourceMap;
            private set
            {
                if (sourceMap != value)
                {
                    sourceMap = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NumRows));
                    OnPropertyChanged(nameof(NumColumns));
                    ObservableCollection<MapSpaceViewModel> vms = new ObservableCollection<MapSpaceViewModel>();
                    if (SourceMap.Spaces != null)
                    {
                        foreach (MapSpace space in SourceMap.Spaces)
                        {
                            vms.Add(new MapSpaceViewModel(space, this));
                        }
                    }
                    SpaceViewModels = vms;
                }
            }
        }

        public GameInstanceViewModel Parent { get; protected set; }

        public int NumRows
        {
            get 
            {
                return SourceMap == null ? 0 : SourceMap.Height;
            }
        }

        public int NumColumns
        {
            get
            {
                return SourceMap == null ? 0 : SourceMap.Width;
            }
        }

        public ObservableCollection<MapSpaceViewModel> SpaceViewModels
        {
            get => spaceVms;
            set
            {
                if (spaceVms != value)
                {
                    spaceVms = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CuratedSpaceViewModels));
                }
            }
        }

        public ObservableCollection<ObservableCollection<MapSpaceViewModel>> CuratedSpaceViewModels
        {
            get => CurateSpaces();
        }

        public MapSpaceViewModel SelectedSpace
        {
            get => selectedSpace;
            set
            {
                if (selectedSpace != value)
                {
                    MapSpaceViewModel oldValue = selectedSpace;
                    selectedSpace = value;
                    OnPropertyChanged();
                    SelectedSpaceChanged?.Invoke(this, new ValueChangedEventArgs<MapSpaceViewModel>(oldValue, selectedSpace));
                }
            }
        }

        public MapMode SelectedMapMode
        {
            get => selectedMapMode;
            set
            {
                if (selectedMapMode != value)
                {
                    MapMode oldValue = selectedMapMode;
                    selectedMapMode = value;
                    OnPropertyChanged();
                    SelectedMapModeChanged?.Invoke(this, new ValueChangedEventArgs<MapMode>(oldValue, value));
                }
            }
        }
        #endregion

        #region Constructors
        public GameMapViewModel(GameMap sourceMap, GameInstanceViewModel parent)
        {
            Parent = parent;
            SourceMap = sourceMap;
            SelectedMapMode = MapMode.Culture;
        }
        #endregion

        #region Methods
        protected ObservableCollection<ObservableCollection<MapSpaceViewModel>> CurateSpaces()
        {
            if (SpaceViewModels == null)
            {
                return null;
            }
            int spacesIndex;
            ObservableCollection<ObservableCollection<MapSpaceViewModel>> curated = new ObservableCollection<ObservableCollection<MapSpaceViewModel>>();
            for (int col = 0; col < NumColumns; col++)
            {
                curated.Add(new ObservableCollection<MapSpaceViewModel>());
                for (int row = 0; row < NumRows; row++)
                {
                    spacesIndex = row + (col * NumRows);
                    if (spacesIndex < SpaceViewModels.Count)
                    {
                        curated[col].Add(SpaceViewModels[spacesIndex]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return curated;
        }
        #endregion
    }
}
