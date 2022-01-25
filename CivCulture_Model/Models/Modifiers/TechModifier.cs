using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Modifiers
{
    public class TechModifier<TValue> : Modifier<TValue>
    {
        #region Events
        #endregion

        #region Fields
        private readonly TechnologyTemplate technology;
        #endregion

        #region Properties
        public TechnologyTemplate Technology => technology;
        #endregion

        #region Constructors
        public TechModifier(TechnologyTemplate tech, TValue modification) : base(tech.Name, modification) 
        {
            technology = tech;
        }
        #endregion

        #region Methods
        #endregion
    }
}
