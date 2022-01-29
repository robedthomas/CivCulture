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
                    if (spaceVms != null)
                    {
                        foreach (MapSpaceViewModel vm in spaceVms)
                        {
                            vm.SourceSpace.DominantCultureChanged -= Space_DominantCultureChanged;
                        }
                        spaceVms.CollectionChanged -= SpaceViewModels_CollectionChanged;
                    }
                    spaceVms = value;
                    if (spaceVms != null)
                    {
                        foreach (MapSpaceViewModel vm in spaceVms)
                        {
                            vm.SourceSpace.DominantCultureChanged += Space_DominantCultureChanged;
                            UpdateCultureBordersOfSpace(vm.SourceSpace);
                        }
                        spaceVms.CollectionChanged += SpaceViewModels_CollectionChanged;
                    }
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

        private void SpaceViewModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MapSpaceViewModel vm in e.NewItems)
                {
                    vm.SourceSpace.DominantCultureChanged += Space_DominantCultureChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (MapSpaceViewModel vm in e.OldItems)
                {
                    vm.SourceSpace.DominantCultureChanged -= Space_DominantCultureChanged;
                }
            }
        }

        private void Space_DominantCultureChanged(object sender, ValueChangedEventArgs<Culture> e)
        {
            MapSpace changedSpace = sender as MapSpace;
            UpdateCultureBordersOfSpace(changedSpace);
        }

        private void UpdateCultureBordersOfSpace(MapSpace changedSpace)
        {
            // Find spaces to the left, right, up, and down of the changed space
            MapSpace leftSpace = SourceMap.GetSpaceInRelativeDirection(changedSpace, RelativeDirection.Left);
            MapSpace rightSpace = SourceMap.GetSpaceInRelativeDirection(changedSpace, RelativeDirection.Right);
            MapSpace upSpace = SourceMap.GetSpaceInRelativeDirection(changedSpace, RelativeDirection.Up);
            MapSpace downSpace = SourceMap.GetSpaceInRelativeDirection(changedSpace, RelativeDirection.Down);
            // Update border settings between the changed space and each of its adjacent spaces
            UpdateCultureBorderBetweenSpaces(changedSpace, leftSpace, RelativeDirection.Left);
            UpdateCultureBorderBetweenSpaces(changedSpace, rightSpace, RelativeDirection.Right);
            UpdateCultureBorderBetweenSpaces(changedSpace, upSpace, RelativeDirection.Up);
            UpdateCultureBorderBetweenSpaces(changedSpace, downSpace, RelativeDirection.Down);
        }

        private void UpdateCultureBorderBetweenSpaces(MapSpace sourceSpace, MapSpace targetSpace, RelativeDirection directionFromSource)
        {
            MapSpaceViewModel sourceSpaceVm = SpaceViewModels.First(vm => vm.SourceSpace == sourceSpace);
            MapSpaceViewModel targetSpaceVm = targetSpace is null ? null : SpaceViewModels.First(vm => vm.SourceSpace == targetSpace);
            if (targetSpace != null && sourceSpace.DominantCulture == targetSpace.DominantCulture)
            {
                RemoveCultureBorderBetweenSpaces(sourceSpaceVm, targetSpaceVm, directionFromSource);
            }
            else
            {
                AddCultureBorderBetweenSpaces(sourceSpaceVm, targetSpaceVm, directionFromSource);
            }
        }

        private void AddCultureBorderBetweenSpaces(MapSpaceViewModel sourceSpace, MapSpaceViewModel targetSpace, RelativeDirection directionFromSource)
        {
            bool addBorderToSource = sourceSpace.SourceSpace.DominantCulture != null;
            bool addBorderToTarget = targetSpace is null ? false : targetSpace.SourceSpace.DominantCulture != null;
            switch (directionFromSource)
            {
                case RelativeDirection.Left:
                    sourceSpace.HasCultureBorderLeft = addBorderToSource;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderRight = addBorderToTarget;
                    }
                    break;
                case RelativeDirection.Right:
                    sourceSpace.HasCultureBorderRight = addBorderToSource;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderLeft = addBorderToTarget;
                    }
                    break;
                case RelativeDirection.Up:
                    sourceSpace.HasCultureBorderUp = addBorderToSource;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderDown = addBorderToTarget;
                    }
                    break;
                case RelativeDirection.Down:
                    sourceSpace.HasCultureBorderDown = addBorderToSource;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderUp = addBorderToTarget;
                    }
                    break;
            }
        }

        private void RemoveCultureBorderBetweenSpaces(MapSpaceViewModel sourceSpace, MapSpaceViewModel targetSpace, RelativeDirection directionFromSource)
        {
            switch (directionFromSource)
            {
                case RelativeDirection.Left:
                    sourceSpace.HasCultureBorderLeft = false;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderRight = false;
                    }
                    break;
                case RelativeDirection.Right:
                    sourceSpace.HasCultureBorderRight = false;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderLeft = false;
                    }
                    break;
                case RelativeDirection.Up:
                    sourceSpace.HasCultureBorderUp = false;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderDown = false;
                    }
                    break;
                case RelativeDirection.Down:
                    sourceSpace.HasCultureBorderDown = false;
                    if (targetSpace != null)
                    {
                        targetSpace.HasCultureBorderUp = false;
                    }
                    break;
            }
        }
        #endregion
    }
}
