using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Fundamental : Consumeable
    {
        #region Static Members
        public static Fundamental Food;
        public static Fundamental Production;
        public static Fundamental Money;
        public static Fundamental Luxuries;

        public static void InitializeFundamentals()
        {
            Food = new Fundamental("Food");
            Production = new Fundamental("Production");
            Money = new Fundamental("Money");
            Luxuries = new Fundamental("Luxuries");
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
        public Fundamental(string name)
        {
            Name = name;
        }
        #endregion

        #region Methods
        #endregion
    }
}
