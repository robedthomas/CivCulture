using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Modifiers
{
    public class DecimalModifiable : Modifiable<decimal>
    {
        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public override decimal AggregateModifiers(IEnumerable<Modifier<decimal>> modifiers, IEnumerable<Modifier<decimal>> factorModifiers)
        {
            decimal aggregation = 0;
            foreach (Modifier<decimal> mod in modifiers)
            {
                aggregation += mod.Modification;
            }
            decimal factor = 1;
            foreach (Modifier<decimal> factorMod in factorModifiers)
            {
                factor += factorMod.Modification;
            }
            return aggregation * factor;
        }
        #endregion
    }
}
