using CivCulture_Model.Models;
using CivCulture_Model.Models.MetaComponents;
using CivCulture_Model.Models.MetaComponents.MapGenerations;
using CivCulture_Model.Models.MetaComponents.TurnLogics;
using CivCulture_ViewModel.Utilities;
using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CivCulture_ViewModel.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        private GameConfigViewModel configVM;
        private GameInstanceViewModel instanceVM;
        private RelayCommand newGameRC;
        private Visibility menuVisibility;
        private Visibility configVisibility;
        private Visibility gameVisibility;
        #endregion

        #region Events
        #endregion

        #region Properties
        public MainModel MainModel { get; set; }

        public GameConfigViewModel GameConfigVM
        {
            get => configVM;
            set
            {
                if (configVM != value)
                {
                    configVM = value;
                    OnPropertyChanged();
                }
            }
        }

        public GameInstanceViewModel GameInstanceVM
        {
            get => instanceVM;
            set
            {
                if (instanceVM != value)
                {
                    instanceVM = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand NewGameRC
        {
            get => newGameRC;
            set
            {
                if (newGameRC != value)
                {
                    newGameRC = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility MenuVisibility
        {
            get => menuVisibility;
            set
            {
                if (menuVisibility != value)
                {
                    menuVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility ConfigVisibility
        {
            get => configVisibility;
            set
            {
                if (configVisibility != value)
                {
                    configVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility GameVisibility
        {
            get => gameVisibility;
            set
            {
                if (gameVisibility != value)
                {
                    gameVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            MenuVisibility = Visibility.Visible;
            ConfigVisibility = Visibility.Collapsed;
            GameVisibility = Visibility.Collapsed;
            MainModel = new MainModel();
            GameConfigVM = new GameConfigViewModel(this);
            NewGameRC = new RelayCommand((p) => ConfigureNewGame());
        }
        #endregion

        #region Methods
        private void ConfigureNewGame()
        {
            MenuVisibility = Visibility.Collapsed;
            ConfigVisibility = Visibility.Visible;
            GameVisibility = Visibility.Collapsed;
        }

        public void GenerateNewGame(MapGeneration generation, MapConfiguration config, TurnLogic logic)
        {
            MainModel.CurrentGame = MainModel.GenerateNewGame(generation, config, logic);
            GameInstanceVM = new GameInstanceViewModel(MainModel.CurrentGame);
            MenuVisibility = Visibility.Collapsed;
            ConfigVisibility = Visibility.Collapsed;
            GameVisibility = Visibility.Visible;
        }
        #endregion
    }
}
