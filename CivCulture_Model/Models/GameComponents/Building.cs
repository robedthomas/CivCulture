using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Building : JobSource
    {
        #region Events
        public ValueChangedEventHandler<BuildingTemplate> TemplateChanged;
        public ValueChangedEventHandler<MapSpace> SpaceChanged;
        public ValueChangedEventHandler<decimal> CompletionLevelChanged;
        #endregion

        #region Fields
        private BuildingTemplate template;
        private MapSpace space;
        private decimal completionLevel;
        #endregion

        #region Properties
        public BuildingTemplate Template
        {
            get => template;
            set
            {
                if (template != value)
                {
                    BuildingTemplate oldValue = template;
                    template = value;
                    TemplateChanged?.Invoke(this, new ValueChangedEventArgs<BuildingTemplate>(oldValue, value));
                }
            }
        }

        public MapSpace Space
        {
            get => space;
            set
            {
                if (space != value)
                {
                    MapSpace oldValue = space;
                    space = value;
                    SpaceChanged?.Invoke(this, new ValueChangedEventArgs<MapSpace>(oldValue, value));
                }
            }
        }

        public decimal CompletionLevel
        {
            get => completionLevel;
            set
            {
                if (value > 1.0M)
                {
                    value = 1.0M;
                }
                else if (value < 0.0M)
                {
                    value = 0.0M;
                }
                if (completionLevel != value)
                {
                    decimal oldValue = completionLevel;
                    completionLevel = value;
                    CompletionLevelChanged?.Invoke(this, new ValueChangedEventArgs<decimal>(oldValue, value));
                }
            }
        }

        public bool IsComplete
        {
            get => CompletionLevel == 1.0M;
        }
        #endregion

        #region Constructors
        public Building(BuildingTemplate template, MapSpace space)
        {
            Template = template;
            Space = space;
            CompletionLevel = 0;
        }
        #endregion

        #region Methods
        #endregion
    }
}
