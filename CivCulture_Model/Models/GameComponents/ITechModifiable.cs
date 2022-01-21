using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    /// <summary>
    /// Represents a component whose properties can be modified by the techs researched by their culture
    /// </summary>
    public interface ITechModifiable
    {
        /// <summary>
        /// The component whose researched technologies apply modifiers to this component
        /// </summary>
        ITechResearcher TechSource { get; }

        /// <summary>
        /// The collection of all modifiers from technology that apply to this component
        /// </summary>
        TechModifierCollection TechModifiers { get; }
    }
}
