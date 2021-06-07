using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Fundamental : GameComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; protected set; }
        #endregion

        #region Constructors
        public Fundamental(string name)
        {
            Name = name;
        }
        #endregion

        #region Methods
        #endregion
    }
}
