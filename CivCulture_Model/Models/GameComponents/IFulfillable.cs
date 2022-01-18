using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    /// <summary>
    /// Represents a component that requires a certain cost to be payed in order to be fulfilled
    /// </summary>
    /// <typeparam name="TCostType">The type of the cost</typeparam>
    public interface IFulfillable<TCostType>
    {
        /// <summary>
        /// The total costs to fulfill this object
        /// </summary>
        public TCostType TotalCosts { get; }

        /// <summary>
        /// The costs not yet paid
        /// </summary>
        public TCostType RemainingCosts { get; }

        /// <summary>
        /// The amount of completion of this object's costs. 0 implies no costs have been paid, 1 implies all costs have been paid
        /// </summary>
        public decimal CompletionLevel { get; }
    }
}
