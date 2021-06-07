using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture.ViewModels
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
            GameMap map = new GameMap(1, 1);
            map.Spaces[0, 0] = new MapSpace(0, 0, new Terrain("Grassland"));
            MapVM = new GameMapViewModel(map);
        }
        #endregion
    }
}
