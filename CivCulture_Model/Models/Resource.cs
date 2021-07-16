using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;

namespace CivCulture_Model.Models
{
    public class Resource : Consumeable
    {
        #region Static Members
        public static Resource Wheat;

        public static void InitializeResources()
        {
            Fundamental.InitializeFundamentals();
            Wheat = new Resource("Wheat", new Tuple<Fundamental, decimal>(Fundamental.Food, 2M));
        }
        #endregion

        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; private set; }

        public FundamentalsCollection FundementalValues { get; private set; } = new FundamentalsCollection();
        #endregion

        #region Constructors
        public Resource(string name, params Tuple<Fundamental, decimal>[] fundamentalValues)
        {
            Name = name;
            foreach (Tuple<Fundamental, decimal> pair in fundamentalValues)
            {
                FundementalValues.Add(pair);
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
