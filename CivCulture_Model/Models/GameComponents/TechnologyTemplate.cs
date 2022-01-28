using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class TechnologyTemplate : ComponentTemplate
    {
        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public ConsumeablesCollection Costs { get; protected set; }

        public HashSet<TechnologyTemplate> Parents { get; protected set; }

        public HashSet<TechnologyTemplate> Children { get; protected set; }

        public TechModifierCollection Modifiers { get; protected set; }

        public ObservableCollection<Culture> CulturesResearchedBy { get; protected set; }
        #endregion

        #region Constructors
        public TechnologyTemplate(string name, ConsumeablesCollection costs)
        {
            Name = name;
            Costs = new ConsumeablesCollection(costs);
            Parents = new HashSet<TechnologyTemplate>();
            Children = new HashSet<TechnologyTemplate>();
            Modifiers = new TechModifierCollection();
            CulturesResearchedBy = new ObservableCollection<Culture>();
        }
        #endregion

        #region Methods
        #endregion
    }
}
