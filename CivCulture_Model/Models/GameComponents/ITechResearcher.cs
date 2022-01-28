using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    /// <summary>
    /// Represents a component that can research technologies
    /// </summary>
    public interface ITechResearcher
    {
        /// <summary>
        /// The technology currently being researched by this ITechResearcher
        /// </summary>
        Technology CurrentResearch { get; }

        /// <summary>
        /// All technologies that have been researched by this ITechResearcher
        /// </summary>
        ObservableCollection<Technology> ResearchedTechnologies { get; }

        /// <summary>
        /// All technologies that may currently be researched by this ITechResearcher
        /// </summary>
        ObservableCollection<Technology> AvailableTechnologies { get; }

        /// <summary>
        /// All modifiers from technologies that have been researched by this ITechResearcher
        /// </summary>
        TechModifierCollection TechModifiers { get; }
    }
}
