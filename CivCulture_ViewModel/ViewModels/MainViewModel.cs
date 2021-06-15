using CivCulture_Model.Models;
using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
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
            MakeExampleMap();
        }
        #endregion

        #region Methods
        public void MakeExampleMap()
        {
            GameMap map = new GameMap(2, 2);
            map.Spaces[0, 0] = new MapSpace(0, 0, new Terrain("Grassland"));
            map.Spaces[0, 1] = new MapSpace(0, 1, new Terrain("Mountain"));
            map.Spaces[1, 0] = new MapSpace(1, 0, new Terrain("Plains"));
            map.Spaces[1, 1] = new MapSpace(1, 1, null);
            MapVM = new GameMapViewModel(map);
        }
        #endregion
    }
}
