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
            Food = new Fundamental("Food", 1);
            Production = new Fundamental("Production", 3);
            Money = new Fundamental("Money", 1);
            Luxuries = new Fundamental("Luxuries", 6);
            PopTemplate.InitializePopTemplates();
        }
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public Fundamental(string name, decimal baseValue) : base(name, baseValue)
        {
        }
        #endregion

        #region Methods
        #endregion
    }
}
