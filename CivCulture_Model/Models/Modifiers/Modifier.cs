using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.Modifiers
{
    public class Modifier<TValue>
    {
        #region Events
        #endregion

        #region Fields
        private readonly TValue modification;
        private readonly string name;
        #endregion

        #region Properties
        public TValue Modification => modification;

        public string Name => name;
        #endregion

        #region Constructors
        public Modifier(string name, TValue modification)
        {
            this.name = name;
            this.modification = modification;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            string modVal;
            if (modification is int intMod)
            {
                if (intMod >= 0)
                {
                    modVal = $"+{intMod}";
                }
                else
                {
                    modVal = $"{intMod}";
                }
            }
            else if (modification is decimal decMod)
            {
                if (decMod >= 0)
                {
                    modVal = $"+{decMod}";
                }
                else
                {
                    modVal = $"{decMod}";
                }
            }
            else
            {
                modVal = modification.ToString();
            }
            return $"({modVal}) {Name}";
        }
        #endregion
    }
}
