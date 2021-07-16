using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Terrain : GameComponent
    {
        #region Static Members
        public static Terrain Grassland;
        public static Terrain Plains;
        public static Terrain Mountains;

        static Terrain()
        {
            Grassland = new Terrain("Grassland");
            Plains = new Terrain("Plains");
            Mountains = new Terrain("Mountains");
        }
        #endregion

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
