using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("{Name} Resource")]
    public class Resource : Consumeable
    {
        #region Static Members
        public static Resource Wheat;
        public static Resource Wood;

        public static void InitializeResources()
        {
            Fundamental.InitializeFundamentals();
            Wood = new Resource("Wood", 1);
        }
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public Resource(string name, int baseValue, bool accumulates = true) : base(name, baseValue, accumulates: accumulates)
        {
        }
        #endregion

        #region Methods
        #endregion
    }
}
