using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class MapSpaceDetailsViewModel : BaseViewModel
    {
        #region Fields
        public const int MAX_POPS_PER_ROW = 8;

        private MapSpace sourceSpace;
        private string spaceName;
        private ObservableCollection<PopViewModel> popVMs;
        private PopViewModel selectedPop;
        #endregion

        #region Events
        #endregion

        #region Properties
        public MapSpace SourceSpace
        {
            get => sourceSpace;
            set
            {
                if (sourceSpace != value)
                {
                    PopViewModels = new ObservableCollection<PopViewModel>();
                    if (sourceSpace != null)
                    {
                        UnsubscribeFromSourceSpaceEvents();
                    }
                    sourceSpace = value;
                    if (sourceSpace != null)
                    {
                        SubscribeToSourceSpaceEvents();
                        foreach (Pop pop in sourceSpace.Pops)
                        {
                            PopViewModels.Add(new PopViewModel() { SourcePop = pop });
                        }
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SpaceRow));
                    OnPropertyChanged(nameof(SpaceColumn));
                    OnPropertyChanged(nameof(TerrainName));
                    OnPropertyChanged(nameof(NumPopRows));
                    OnPropertyChanged(nameof(NumPopColumns));
                    SpaceName = SourceSpace.Name;
                }
            }
        }

        public string SpaceName
        {
            get => spaceName;
            set
            {
                if (spaceName != value)
                {
                    spaceName = value;
                    SourceSpace.Name = SpaceName;
                    OnPropertyChanged();
                }
            }
        }

        public int SpaceRow
        {
            get => sourceSpace.Row;
        }

        public int SpaceColumn
        {
            get => sourceSpace.Column;
        }

        public string TerrainName
        {
            get => sourceSpace.Terrain?.Name;
        }

        public ObservableCollection<PopViewModel> PopViewModels
        {
            get => popVMs;
            set
            {
                if (popVMs != value)
                {
                    popVMs = value;
                    OnPropertyChanged();
                }
            }
        }

        public PopViewModel SelectedPop
        {
            get => selectedPop;
            set
            {
                if (selectedPop != value)
                {
                    selectedPop = value;
                    OnPropertyChanged();
                }
            }
        }

        public int NumPopRows
        {
            get => (int)Math.Ceiling(decimal.Divide(PopViewModels.Count, MAX_POPS_PER_ROW));
        }

        public int NumPopColumns
        {
            get => MAX_POPS_PER_ROW;
        }
        #endregion

        #region Constructors
        public MapSpaceDetailsViewModel(MapSpace sourceSpace)
        {
            SourceSpace = sourceSpace;
        }
        #endregion

        #region Methods
        private void UnsubscribeFromSourceSpaceEvents()
        {
            SourceSpace.Pops.CollectionChanged -= SourceSpace_Pops_CollectionChanged;
            SourceSpace.Jobs.CollectionChanged -= SourceSpace_Jobs_CollectionChanged;
            SourceSpace.TerrainResources.CollectionChanged -= SourceSpace_TerrainResources_CollectionChanged;
            SourceSpace.TerrainChanged -= SourceSpace_TerrainChanged;
        }

        private void SubscribeToSourceSpaceEvents()
        {
            SourceSpace.Pops.CollectionChanged += SourceSpace_Pops_CollectionChanged;
            SourceSpace.Jobs.CollectionChanged += SourceSpace_Jobs_CollectionChanged;
            SourceSpace.TerrainResources.CollectionChanged += SourceSpace_TerrainResources_CollectionChanged;
            SourceSpace.TerrainChanged += SourceSpace_TerrainChanged;
        }

        private void SourceSpace_TerrainChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<Terrain> e)
        {
            OnPropertyChanged(nameof(TerrainName));
        }

        private void SourceSpace_Pops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Pop oldPop in e.OldItems)
                {
                    PopViewModel oldVM = PopViewModels.First((vm) => vm.SourcePop == oldPop);
                    if (oldVM != null)
                    {
                        PopViewModels.Remove(oldVM);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (Pop newPop in e.NewItems)
                {
                    PopViewModels.Add(new PopViewModel() { SourcePop = newPop });
                }
            }
            OnPropertyChanged(nameof(NumPopRows));
            OnPropertyChanged(nameof(NumPopColumns));
        }

        private void SourceSpace_Jobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // @TODO: define when JobViewModel is created
        }

        private void SourceSpace_TerrainResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // @TODO: define when TerrainResourceViewModel is created
        }
        #endregion
    }
}
