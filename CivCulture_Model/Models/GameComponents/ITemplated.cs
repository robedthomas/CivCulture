using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    /// <summary>
    /// Represents a component that is an instance of a template class
    /// </summary>
    /// <typeparam name="TTemplate">The template class that this component is an instance of</typeparam>
    public interface ITemplated<TTemplate> where TTemplate : ComponentTemplate
    {
        /// <summary>
        /// The template that this is an instance of
        /// </summary>
        TTemplate Template { get; set; }
    }
}
