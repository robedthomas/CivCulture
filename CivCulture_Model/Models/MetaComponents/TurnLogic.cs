using CivCulture_Model.Models.MetaComponents.UserMutables;
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
        public abstract void InitGameInstance(GameInstance instance, NamesDatabase namesDb);

        public abstract void ExecuteGameTurn(GameInstance instance, NamesDatabase namesDb);
        #endregion
    }
}
