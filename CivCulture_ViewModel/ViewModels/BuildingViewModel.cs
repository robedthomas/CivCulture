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
                    sourceBuilding = value;
                    OnPropertyChanged();
                }
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
        #endregion
    }
}
