using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class MapSpaceDetailsViewModel : BaseViewModel
    {
        #region Fields
        private MapSpace sourceSpace;
        private string spaceName;
        #endregion

        #region Events
        #endregion

        #region Properties
        public MapSpace SourceSpace
        {
            get => sourceSpace;
            set
            {
                if (sourceSpace != value)
                {
                    sourceSpace = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SpaceRow));
                    OnPropertyChanged(nameof(SpaceColumn));
                    SpaceName = SourceSpace.Name;
                }
            }
        }

        public string SpaceName
        {
            get => spaceName;
            set
            {
                if (spaceName != value)
                {
                    spaceName = value;
                    SourceSpace.Name = SpaceName;
                    OnPropertyChanged();
                }
            }
        }

        public int SpaceRow
        {
            get => sourceSpace.Row;
        }

        public int SpaceColumn
        {
            get => sourceSpace.Column;
        }
        #endregion

        #region Constructors
        public MapSpaceDetailsViewModel(MapSpace sourceSpace)
        {
            SourceSpace = sourceSpace;
        }
        #endregion

        #region Methods
        #endregion
    }
}
