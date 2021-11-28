using CivCulture_Model.Models.MetaComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class MainModel : BaseModel
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public GameInstance CurrentGame { get; set; }
        #endregion

        #region Constructors
        public MainModel()
        {
        }
        #endregion

        #region Methods
        public static GameInstance GenerateNewGame(MapGeneration mapGen, MapConfiguration mapConfig, TurnLogic turnLogic)
        {
            GameInstance output = new GameInstance() { MapGeneration = mapGen, MapConfig = mapConfig, TurnLogic = turnLogic };
            output.GenerateMap();
            turnLogic.InitGameInstance(output);
            return output;
        }
        #endregion
    }
}
