using CivCulture_Model.Models;
using CivCulture_Model.Models.MetaComponents;
using CivCulture_Model.Models.MetaComponents.MapGenerations;
using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        private GameMapViewModel mapVM;
        #endregion

        #region Events
        #endregion

        #region Properties
        public MainModel MainModel { get; set; }

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
        public MainViewModel()
        {
            MainModel = new MainModel();
            MainModel.CurrentGame = MainModel.GenerateNewGame(new TestMapGeneration(), new MapConfiguration() { Height = 2, Width = 2 });
            MainModel.CurrentGame.GenerateMap();
            MapVM = new GameMapViewModel(MainModel.CurrentGame.Map);
        }
        #endregion

        #region Methods
        #endregion
    }
}
