using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class BuildingSlotViewModel : BaseViewModel
    {
        #region Events
        #endregion

        #region Fields
        private BuildingSlot sourceResource;
        #endregion

        #region Properties
        public BuildingSlot SourceSlot
        {
            get => sourceResource;
            set
            {
                if (sourceResource != value)
                {
                    sourceResource = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Name
        {
            get => SourceSlot.Template.DisplayName;
        }
        #endregion

        #region Constructors
        public BuildingSlotViewModel(BuildingSlot sourceSlot)
        {
            SourceSlot = sourceSlot;
        }
        #endregion

        #region Methods
        #endregion
    }
}
