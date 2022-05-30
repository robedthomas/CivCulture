using CivCulture_Model.Events;
using CivCulture_Model.Models;
using CivCulture_Model.Models.Collections;
using CivCulture_ViewModel.Utilities;
using GenericUtilities.Observables;
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
        private ObservableCollection<BuildingViewModel> buildingViewModels;
        private BuildingViewModel currentconstructionVM;
        private PopViewModel selectedPop;
        private ObservableDictionary<JobTemplate, JobGroupViewModel> allJobGroups;
        private ConsumeablesCollection totalOutput;
        #endregion

        #region Events
        #endregion

        #region Properties
        public GameInstanceViewModel GameInstanceVM { get; protected set; }

        public MapSpace SourceSpace
        {
            get => sourceSpace;
            set
            {
                if (sourceSpace != value)
                {
                    AllJobGroups = GetJobGroupsFromSpace(value);
                    PopViewModels = new ObservableCollection<PopViewModel>();
                    BuildingViewModels = new ObservableCollection<BuildingViewModel>();
                    TerrainResourceVMs = new ObservableCollection<TerrainResourceViewModel>();
                    CurrentConstructionVM = null;
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
                        foreach (Building building in sourceSpace.Buildings)
                        {
                            BuildingViewModels.Add(new BuildingViewModel(building));
                        }
                        foreach (TerrainResource resource in sourceSpace.TerrainResources)
                        {
                            TerrainResourceVMs.Add(new TerrainResourceViewModel(resource));
                        }
                        if (sourceSpace.CurrentConstruction != null)
                        {
                            CurrentConstructionVM = new BuildingViewModel(sourceSpace.CurrentConstruction);
                        }
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SpaceRow));
                    OnPropertyChanged(nameof(SpaceColumn));
                    OnPropertyChanged(nameof(TerrainName));
                    OnPropertyChanged(nameof(NumPopRows));
                    OnPropertyChanged(nameof(NumPopColumns));
                    OnPropertyChanged(nameof(StockpileResources));
                    OnPropertyChanged(nameof(StockpileMoney));
                    OnPropertyChanged(nameof(DominantCulture));
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

        public ObservableCollection<BuildingViewModel> BuildingViewModels
        {
            get => buildingViewModels;
            set
            {
                if (buildingViewModels != value)
                {
                    buildingViewModels = value;
                    OnPropertyChanged();
                }
            }
        }

        public BuildingViewModel CurrentConstructionVM
        {
            get => currentconstructionVM;
            set
            {
                if (currentconstructionVM != value)
                {
                    currentconstructionVM = value;
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

        public Culture DominantCulture
        {
            get => SourceSpace.DominantCulture;
        }

        public decimal PopGrowthProgress
        {
            get => SourceSpace.PopGrowthProgress;
        }

        public PopTemplate NextPopTemplate
        {
            get => SourceSpace.NextPopTemplate;
        }

        public int FilledJobCount
        {
            get => SourceSpace.Jobs.Where(job => job.Worker != null).Count();
        }

        public int TotalJobCount
        {
            get => SourceSpace.Jobs.Count;
        }

        public ObservableDictionary<JobTemplate, JobGroupViewModel> AllJobGroups
        {
            get => allJobGroups;
            set
            {
                if (allJobGroups != value)
                {
                    allJobGroups = value;
                    OnPropertyChanged();
                }
            }
        }

        public ConsumeablesCollection TotalOutput
        {
            get => totalOutput;
            set
            {
                if (totalOutput != value)
                {
                    totalOutput = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<TerrainResourceViewModel> TerrainResourceVMs { get; protected set; }

        public ConsumeablesCollection StockpileResources
        {
            get => SourceSpace.OwnedResources;
        }

        public decimal StockpileMoney
        {
            get => SourceSpace.Money;
        }

        public ConsumeablesCollection MarketDemandedResources
        {
            get => SourceSpace.ResourceMarket.DemandedResources;
        }

        public ConsumeablesCollection MarketSuppliedResources
        {
            get => SourceSpace.ResourceMarket.SuppliedResources;
        }

        public ConsumeablesCollection MarketResourcePrices
        {
            get => SourceSpace.ResourceMarket.ResourcePrices;
        }
        #endregion

        #region Constructors
        public MapSpaceDetailsViewModel(MapSpace sourceSpace, GameInstanceViewModel gameInstanceVM)
        {
            SourceSpace = sourceSpace;
            TotalOutput = SourceSpace.GetTotalOutput();
            GameInstanceVM = gameInstanceVM;
        }
        #endregion

        #region Methods
        private void UnsubscribeFromSourceSpaceEvents()
        {
            SourceSpace.Pops.CollectionChanged -= SourceSpace_Pops_CollectionChanged;
            SourceSpace.Jobs.CollectionChanged -= SourceSpace_Jobs_CollectionChanged;
            SourceSpace.Buildings.CollectionChanged -= SourceSpace_Buildings_CollectionChanged;
            SourceSpace.CurrentConstructionChanged -= SourceSpace_CurrentConstructionChanged;
            SourceSpace.TerrainResources.CollectionChanged -= SourceSpace_TerrainResources_CollectionChanged;
            SourceSpace.TerrainChanged -= SourceSpace_TerrainChanged;
            SourceSpace.DominantCultureChanged -= SourceSpace_DominantCultureChanged;
            sourceSpace.PopGrowthProgressChanged -= SourceSpace_PopGrowthProgressChanged;
            sourceSpace.NextPopTemplateChanged -= SourceSpace_NextPopTemplateChanged;
            sourceSpace.OwnedResourcesChanged -= SourceSpace_OwnedResourcesChanged;
            sourceSpace.MoneyChanged -= SourceSpace_MoneyChanged;
            sourceSpace.ResourceMarket.DemandedResourcesChanged -= SourceSpace_Market_DemandedResourcesChanged;
            sourceSpace.ResourceMarket.SuppliedResourcesChanged -= SourceSpace_Market_SuppliedResourcesChanged;
            foreach (Job job in SourceSpace.Jobs)
            {
                job.WorkerChanged -= Job_WorkerChanged;
            }
        }

        private void SubscribeToSourceSpaceEvents()
        {
            SourceSpace.Pops.CollectionChanged += SourceSpace_Pops_CollectionChanged;
            SourceSpace.Jobs.CollectionChanged += SourceSpace_Jobs_CollectionChanged;
            SourceSpace.Buildings.CollectionChanged += SourceSpace_Buildings_CollectionChanged;
            SourceSpace.CurrentConstructionChanged += SourceSpace_CurrentConstructionChanged;
            SourceSpace.TerrainResources.CollectionChanged += SourceSpace_TerrainResources_CollectionChanged;
            SourceSpace.TerrainChanged += SourceSpace_TerrainChanged;
            SourceSpace.DominantCultureChanged += SourceSpace_DominantCultureChanged;
            sourceSpace.PopGrowthProgressChanged += SourceSpace_PopGrowthProgressChanged;
            sourceSpace.NextPopTemplateChanged += SourceSpace_NextPopTemplateChanged;
            sourceSpace.OwnedResourcesChanged += SourceSpace_OwnedResourcesChanged;
            sourceSpace.MoneyChanged += SourceSpace_MoneyChanged;
            sourceSpace.ResourceMarket.DemandedResourcesChanged += SourceSpace_Market_DemandedResourcesChanged;
            sourceSpace.ResourceMarket.SuppliedResourcesChanged += SourceSpace_Market_SuppliedResourcesChanged;
            foreach (Job job in SourceSpace.Jobs)
            {
                job.WorkerChanged += Job_WorkerChanged;
            }
        }

        private ObservableDictionary<JobTemplate, JobGroupViewModel> GetJobGroupsFromSpace(MapSpace space)
        {
            if (space == null)
            {
                return null;
            }
            ObservableDictionary<JobTemplate, JobGroupViewModel> output = new ObservableDictionary<JobTemplate, JobGroupViewModel>();
            foreach (Job job in space.Jobs)
            {
                if (!output.ContainsKey(job.Template))
                {
                    output.Add(job.Template, new JobGroupViewModel(job.Template));
                }
                output[job.Template].Jobs.Add(new JobViewModel(job));
            }
            return output;
        }

        public void OpenDominantCultureView()
        {
            OpenCultureView(DominantCulture);
        }

        private void OpenCultureView(Culture cultureToSelect)
        {
            GameInstanceVM.CurrentSelectedCulture = GameInstanceVM.CultureVMs.First(cvm => cvm.SourceCulture == cultureToSelect);
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
            if (e.NewItems != null)
            {
                foreach (Job newJob in e.NewItems)
                {
                    if (!AllJobGroups.ContainsKey(newJob.Template))
                    {
                        AllJobGroups.Add(newJob.Template, new JobGroupViewModel(newJob.Template));
                    }
                    AllJobGroups[newJob.Template].Jobs.Add(new JobViewModel(newJob));
                    newJob.WorkerChanged += Job_WorkerChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Job oldJob in e.OldItems)
                {
                    JobViewModel targetVM = null;
                    foreach (JobViewModel jobVM in AllJobGroups[oldJob.Template].Jobs)
                    {
                        if (jobVM.SourceJob == oldJob)
                        {
                            targetVM = jobVM;
                            break;
                        }
                    }
                    if (targetVM != null)
                    {
                        AllJobGroups[oldJob.Template].Jobs.Remove(targetVM);
                    }
                    oldJob.WorkerChanged -= Job_WorkerChanged;
                }
            }
            TotalOutput = SourceSpace.GetTotalOutput();
            OnPropertyChanged(nameof(FilledJobCount));
            OnPropertyChanged(nameof(TotalJobCount));
        }

        private void SourceSpace_Buildings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Building oldBuilding in e.OldItems)
                {
                    BuildingViewModel oldVM = BuildingViewModels.First((vm) => vm.SourceBuilding == oldBuilding);
                    if (oldVM != null)
                    {
                        BuildingViewModels.Remove(oldVM);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (Building newBuilding in e.NewItems)
                {
                    BuildingViewModels.Add(new BuildingViewModel(newBuilding));
                }
            }
            TotalOutput = SourceSpace.GetTotalOutput();
        }

        private void SourceSpace_CurrentConstructionChanged(object sender, ValueChangedEventArgs<Building> e)
        {
            if (e.NewValue is null)
            {
                CurrentConstructionVM = null;
            }
            else
            {
                CurrentConstructionVM = new BuildingViewModel(e.NewValue);
            }
        }

        private void Job_WorkerChanged(object sender, ValueChangedEventArgs<Pop> e)
        {
            OnPropertyChanged(nameof(FilledJobCount));
            TotalOutput = SourceSpace.GetTotalOutput();
        }

        private void SourceSpace_TerrainResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // @TODO: define when TerrainResourceViewModel is created
        }

        private void SourceSpace_DominantCultureChanged(object sender, ValueChangedEventArgs<Culture> e)
        {
            OnPropertyChanged(nameof(DominantCulture));
        }

        private void SourceSpace_NextPopTemplateChanged(object sender, ValueChangedEventArgs<PopTemplate> e)
        {
            OnPropertyChanged(nameof(NextPopTemplate));
        }

        private void SourceSpace_PopGrowthProgressChanged(object sender, ValueChangedEventArgs<decimal> e)
        {
            OnPropertyChanged(nameof(PopGrowthProgress));
        }

        private void SourceSpace_MoneyChanged(object sender, ValueChangedEventArgs<decimal> e)
        {
            OnPropertyChanged(nameof(StockpileMoney));
        }

        private void SourceSpace_OwnedResourcesChanged(object sender, ValueChangedEventArgs<ConsumeablesCollection> e)
        {
            OnPropertyChanged(nameof(StockpileResources));
        }

        private void SourceSpace_Market_DemandedResourcesChanged(object sender, ValueChangedEventArgs<ConsumeablesCollection> e)
        {
            OnPropertyChanged(nameof(MarketDemandedResources));
        }

        private void SourceSpace_Market_SuppliedResourcesChanged(object sender, ValueChangedEventArgs<ConsumeablesCollection> e)
        {
            OnPropertyChanged(nameof(MarketSuppliedResources));
        }
        #endregion
    }
}
