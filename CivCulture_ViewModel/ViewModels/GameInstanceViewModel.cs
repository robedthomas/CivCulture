using CivCulture_Model.Models;
using CivCulture_Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class GameInstanceViewModel : BaseViewModel
    {
        #region Fields
        private GameMapViewModel mapVM;
        #endregion

        #region Events
        #endregion

        #region Properties
        public GameInstance SourceInstance { get; set; }

        public GameMapViewModel MapVM
        {
            get => mapVM;
            set
            {
                if (mapVM != value)
                {
                    mapVM = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public GameInstanceViewModel(GameInstance sourceInstance)
        {
            SourceInstance = sourceInstance;
        }
        #endregion

        #region Methods
        public void GenerateNewMap()
        {
            SourceInstance.GenerateMap();
            MapVM = new GameMapViewModel(SourceInstance.Map);
        }
        #endregion
    }
}
