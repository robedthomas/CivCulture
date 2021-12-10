using CivCulture_Model.Models.MetaComponents;
using CivCulture_Model.Models.MetaComponents.MapGenerations;
using CivCulture_Model.Models.MetaComponents.TurnLogics;
using CivCulture_ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class GameConfigViewModel : BaseViewModel
    {
        #region Events
        private int mapWidth;
        private int mapHeight;
        private Tuple<string, MapGeneration> selectedMapGeneration;
        private Tuple<string, TurnLogic> selectedTurnLogic;
        private RelayCommand startGameRC;
        #endregion

        #region Fields
        public int DEFAULT_MAP_WIDTH = 10;
        public int DEFAULT_MAP_HEIGHT = 10;
        #endregion

        #region Properties
        private MainViewModel MainVM { get; set; }

        public int MapWidth
        {
            get => mapWidth;
            set
            {
                if (mapWidth != value)
                {
                    mapWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MapHeight
        {
            get => mapHeight;
            set
            {
                if (mapHeight != value)
                {
                    mapHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Tuple<string, MapGeneration>> AllMapGenerations { get; } = new ObservableCollection<Tuple<string, MapGeneration>>()
        {
            new Tuple<string, MapGeneration>("Test", new TestMapGeneration()),
            new Tuple<string, MapGeneration>("Random", new RandomMapGeneration()),
        };

        public ObservableCollection<Tuple<string, TurnLogic>> AllTurnLogics { get; } = new ObservableCollection<Tuple<string, TurnLogic>>()
        {
            new Tuple<string, TurnLogic>("Standard Rules", new StandardTurnLogic()),
        };

        public Tuple<string, MapGeneration> SelectedMapGeneration
        {
            get => selectedMapGeneration;
            set
            {
                if (selectedMapGeneration != value)
                {
                    selectedMapGeneration = value;
                    OnPropertyChanged();
                }
            }
        }

        public Tuple<string, TurnLogic> SelectedTurnLogic
        {
            get => selectedTurnLogic;
            set
            {
                if (selectedTurnLogic != value)
                {
                    selectedTurnLogic = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand StartGameRC
        {
            get => startGameRC;
            set
            {
                if (startGameRC != value)
                {
                    startGameRC = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public GameConfigViewModel(MainViewModel mainVM)
        {
            MainVM = mainVM;
            MapWidth = DEFAULT_MAP_WIDTH;
            MapHeight = DEFAULT_MAP_HEIGHT;
            SelectedMapGeneration = AllMapGenerations.First();
            SelectedTurnLogic = AllTurnLogics.First();
            StartGameRC = new RelayCommand((p) => StartGame());
        }
        #endregion

        #region Methods
        private void StartGame()
        {
            MapConfiguration config = new MapConfiguration()
            {
                Width = MapWidth,
                Height = MapHeight
            };
            MainVM.GenerateNewGame(SelectedMapGeneration.Item2, config, SelectedTurnLogic.Item2);
        }
        #endregion
    }
}
