using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("{Name} Fundamental")]
    public class Fundamental : Consumeable
    {
        #region Static Members
        public static HashSet<Fundamental> AllFundamentals = new HashSet<Fundamental>();

        public static Fundamental Food;
        public static Fundamental Production;
        public static Fundamental Money;
        public static Fundamental Shelter;
        public static Fundamental Luxuries;

        public static void InitializeFundamentals()
        {
            Food = new Fundamental("Food", 1);
            Production = new Fundamental("Production", 3);
            Money = new Fundamental("Money", 1);
            Shelter = new Fundamental("Shelter", 3, accumulates: false);
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
        public Fundamental(string name, decimal baseValue, bool accumulates = true) : base(name, baseValue, accumulates: accumulates)
        {
            AllFundamentals.Add(this);
        }
        #endregion

        #region Methods
        #endregion
    }
}
