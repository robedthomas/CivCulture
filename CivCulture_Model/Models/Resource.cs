using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Resource : GameComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public string Name { get; private set; }

        public FundamentalCollection FundementalValues { get; private set; } = new FundamentalCollection();
        #endregion

        #region Constructors
        public Resource(string name, IEnumerable<Tuple<Fundamental, int>> fundamentalValues)
        {
            Name = name;
            foreach (Tuple<Fundamental, int> pair in fundamentalValues)
            {
                FundementalValues.Add(pair);
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
