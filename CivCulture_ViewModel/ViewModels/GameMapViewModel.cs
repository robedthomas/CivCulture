using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.ViewModels
{
    public class GameMapViewModel : BaseViewModel
    {
        #region Fields
        private MapSpaceViewModel spaceVM;
        #endregion

        #region Events
        #endregion

        #region Properties
        public GameMap SourceMap { get; private set; }

        public MapSpaceViewModel SpaceVM
        {
            get => spaceVM;
            set
            {
                if (spaceVM != value)
                {
                    spaceVM = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public GameMapViewModel(GameMap sourceMap)
        {
            SourceMap = sourceMap;
            SpaceVM = new MapSpaceViewModel(SourceMap.Spaces[0, 0]); // @TODO: read in whole map
        }
        #endregion

        #region Methods
        #endregion
    }
}
