using CivCulture_Model.Models;
using CivCulture_ViewModel.Utilities;
using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.ViewModels
{
    public class GameMapViewModel : BaseViewModel
    {
        #region Fields
        private GameMap sourceMap;
        private int numRows, numColumns;
        private ObservableCollection<MapSpaceViewModel> spaceVms;
        private MapSpaceViewModel selectedSpace;
        private MapSpaceDetailsViewModel selectedSpaceDetails;
        #endregion

        #region Events
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
                            vms.Add(new MapSpaceViewModel(space));
                        }
                    }
                    SpaceViewModels = vms;
                }
            }
        }

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
                    selectedSpace = value;
                    OnPropertyChanged();
                    if (SelectedSpace != null)
                    {
                        SelectedSpaceDetails = new MapSpaceDetailsViewModel(SelectedSpace.SourceSpace);
                    }
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
        public GameMapViewModel(GameMap sourceMap)
        {
            SourceMap = sourceMap;
            // @TODO: read in whole map
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
