using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Terrain : GameComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }
        #endregion

        #region Constructors
        public Terrain(string name)
        {
            Name = name;
        }
        #endregion

        #region Methods
        #endregion
    }
}
