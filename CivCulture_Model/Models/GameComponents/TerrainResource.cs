using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("{Template.Name} TerrainResource")]
    public class TerrainResource : JobSource, ITemplated<TerrainResourceTemplate>
    {
        #region Events
        public event ValueChangedEventHandler<TerrainResourceTemplate> TemplateChanged;
        #endregion

        #region Fields
        private TerrainResourceTemplate template;
        #endregion

        #region Properties
        public TerrainResourceTemplate Template
        {
            get => template;
            set
            {
                if (template != value)
                {
                    TerrainResourceTemplate oldValue = template;
                    template = value;
                    TemplateChanged?.Invoke(this, new ValueChangedEventArgs<TerrainResourceTemplate>(oldValue, value));
                }
            }
        }
        #endregion

        #region Constructors
        public TerrainResource(TerrainResourceTemplate template)
        {
            Template = template;
        }
        #endregion

        #region Methods
        #endregion
    }
}
