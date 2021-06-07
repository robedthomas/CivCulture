using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class GameInstance : BaseModel
    {
        #region Fields
        private GameMap map;
        #endregion

        #region Events
        public event ValueChangedEventHandler<GameMap> MapChanged;
        #endregion

        #region Properties
        public GameMap Map
        {
            get
            {
                return map;
            }
            set
            {
                if (map != value)
                {
                    GameMap oldValue = map;
                    map = value;
                    MapChanged?.Invoke(this, new ValueChangedEventArgs<GameMap>(oldValue, map));
                }
            }
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
