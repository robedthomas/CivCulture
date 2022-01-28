using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class TerrainResourceViewModel : BaseViewModel
    {
        #region Events
        #endregion

        #region Fields
        private TerrainResource sourceResource;
        #endregion

        #region Properties
        public TerrainResource SourceResource
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
            get => SourceResource.Template.Name;
        }
        #endregion

        #region Constructors
        public TerrainResourceViewModel(TerrainResource sourceResource)
        {
            SourceResource = sourceResource;
        }
        #endregion

        #region Methods
        #endregion
    }
}
