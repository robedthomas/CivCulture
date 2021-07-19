using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.MetaComponents
{
    public abstract class TurnLogic : MetaComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public abstract void ExecuteGameTurn(GameInstance instance);
        #endregion
    }
}
