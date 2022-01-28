using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class BuildingViewModel : BaseViewModel
    {
        #region Events
        #endregion

        #region Fields
        private Building sourceBuilding;
        #endregion

        #region Properties
        public Building SourceBuilding
        {
            get => sourceBuilding;
            set
            {
                if (sourceBuilding != value)
                {
                    if (sourceBuilding != null)
                    {
                        sourceBuilding.CompletionLevelChanged -= SourceBuilding_CompletionLevelChanged;
                    }
                    sourceBuilding = value;
                    if (sourceBuilding != null)
                    {
                        sourceBuilding.CompletionLevelChanged += SourceBuilding_CompletionLevelChanged;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(ConstructionProgress));
                    OnPropertyChanged(nameof(ConstructionProgressLabel));
                }
            }
        }

        public string Name
        {
            get => SourceBuilding.Template.Name;
        }

        public decimal ConstructionProgress
        {
            get => SourceBuilding.CompletionLevel;
        }

        public string ConstructionProgressLabel
        {
            get
            {
                decimal totalCount = 0, remainingCount = 0;
                foreach (KeyValuePair<Consumeable, decimal> pair in SourceBuilding.TotalCosts)
                {
                    totalCount += pair.Value;
                }
                foreach (KeyValuePair<Consumeable, decimal> pair in SourceBuilding.RemainingCosts)
                {
                    remainingCount += pair.Value;
                }
                decimal completedCount = totalCount - remainingCount;
                return $"{completedCount.ToString("N")} / {totalCount}";
            }
        }
        #endregion

        #region Constructors
        public BuildingViewModel(Building sourceBuilding)
        {
            SourceBuilding = sourceBuilding;
        }
        #endregion

        #region Methods
        private void SourceBuilding_CompletionLevelChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<decimal> e)
        {
            OnPropertyChanged(nameof(ConstructionProgress));
            OnPropertyChanged(nameof(ConstructionProgressLabel));
        }
        #endregion
    }
}
