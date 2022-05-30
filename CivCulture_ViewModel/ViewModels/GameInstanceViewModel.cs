using CivCulture_Model.Events;
using CivCulture_Model.Models;
using CivCulture_ViewModel.Utilities;
using GenericUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CivCulture_ViewModel.ViewModels
{
    public class GameInstanceViewModel : BaseViewModel
    {
        #region Fields
        public readonly static HashSet<Color> STANDARD_CULTURE_COLORS = new HashSet<Color>() { Colors.Red, Colors.Blue, Colors.DarkGreen, Colors.Purple, Colors.LightCyan, Colors.Orange };

        private GameInstance sourceInstance;
        private GameMapViewModel mapVM;
        private MapSpaceDetailsViewModel selectedSpaceDetails;
        private CultureViewModel selectedCultureVM;
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
                        sourceInstance.AllCultures.CollectionChanged -= SourceInstance_AllCultures_CollectionChanged;
                        CultureVMs.Clear();
                    }
                    sourceInstance = value;
                    if (sourceInstance != null)
                    {
                        sourceInstance.MapChanged += SourceInstance_MapChanged;
                        sourceInstance.AllCultures.CollectionChanged += SourceInstance_AllCultures_CollectionChanged;
                        foreach (Culture c in sourceInstance.AllCultures)
                        {
                            CultureVMs.Add(new CultureViewModel(c, RemainingCultureColors.PickRandom(sourceInstance.RandomSeed, removeChoice: true)));
                        }
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

        public ObservableCollection<CultureViewModel> CultureVMs { get; protected set; }

        public CultureViewModel CurrentSelectedCulture
        {
            get => selectedCultureVM;
            set
            {
                if (selectedCultureVM != value)
                {
                    selectedCultureVM = value;
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

        protected HashSet<Color> RemainingCultureColors { get; set; }

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
            RemainingCultureColors = new HashSet<Color>(STANDARD_CULTURE_COLORS);
            CultureVMs = new ObservableCollection<CultureViewModel>();
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
                SelectedSpaceDetails = new MapSpaceDetailsViewModel(e.NewValue.SourceSpace, this);
            }
        }

        private void SourceInstance_MapChanged(object sender, ValueChangedEventArgs<GameMap> e)
        {
            MapVM = new GameMapViewModel(SourceInstance.Map, this);
        }

        private void EndTurn(object param)
        {
            SourceInstance.PassTurn();
        }

        private bool CanEndTurn(object param)
        {
            return true; // @TODO
        }

        private void SourceInstance_AllCultures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Culture newCulture in e.NewItems)
                {
                    CultureVMs.Add(new CultureViewModel(newCulture, RemainingCultureColors.PickRandom(sourceInstance.RandomSeed, removeChoice: true)));
                }
            }
            if (e.OldItems != null)
            {
                foreach (Culture oldCulture in e.OldItems)
                {
                    CultureVMs.Remove(CultureVMs.First(cvm => cvm.SourceCulture == oldCulture));
                }
            }
        }
        #endregion
    }
}
