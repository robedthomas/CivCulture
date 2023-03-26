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

        public static Fundamental Food = new Fundamental("Food", 1);
        public static Fundamental Production = new Fundamental("Production", 3);
        public static Fundamental Money = new Fundamental("Money", 1);
        public static Fundamental Shelter = new Fundamental("Shelter", 3, accumulates: false);
        public static Fundamental Progress = new Fundamental("Progress", 5, cultureWide: true);
        public static Fundamental Luxuries = new Fundamental("Luxuries", 6);
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public Fundamental(string name, decimal baseValue, bool accumulates = true, bool cultureWide = false) : base(name, baseValue, accumulates: accumulates, cultureWide: cultureWide)
        {
            AllFundamentals.Add(this);
        }
        #endregion

        #region Methods
        #endregion
    }
}
