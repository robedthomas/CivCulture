using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.MetaComponents
{
    public class MapConfiguration : MetaComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public int Width { get; set; }
        public int Height { get; set; }

        public int MinimumNumberPops
        {
            get => 1; // @TODO: make configurable
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
