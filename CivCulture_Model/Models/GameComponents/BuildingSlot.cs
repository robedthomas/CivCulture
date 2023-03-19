using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("{Template.Name} BuildingSlot")]
    public class BuildingSlot : JobSource, ITemplated<BuildingSlotTemplate>
    {
        #region Events
        public event ValueChangedEventHandler<BuildingSlotTemplate> TemplateChanged;
        public event ValueChangedEventHandler<Building> OccupyingBuildingChanged;
        public event ValueChangedEventHandler<BuildingSlotTemplate> UnderlyingSlotTemplateChanged;
        #endregion

        #region Fields
        private BuildingSlotTemplate template;
        private Building occupyingBuilding;
        private BuildingSlotTemplate underlyingSlotTemplate;
        #endregion

        #region Properties
        public BuildingSlotTemplate Template
        {
            get => template;
            set
            {
                if (template != value)
                {
                    BuildingSlotTemplate oldValue = template;
                    template = value;
                    TemplateChanged?.Invoke(this, new ValueChangedEventArgs<BuildingSlotTemplate>(oldValue, value));
                }
            }
        }

        public MapSpace ParentSpace { get; }

        public Building OccupyingBuilding
        {
            get => occupyingBuilding;
            set
            {
                if (occupyingBuilding != value)
                {
                    Building oldValue = occupyingBuilding;
                    occupyingBuilding = value;
                    OccupyingBuildingChanged?.Invoke(this, new ValueChangedEventArgs<Building>(oldValue, value));
                }
            }
        }

        public BuildingSlotTemplate UnderlyingSlotTemplate
        {
            get => underlyingSlotTemplate;
            set
            {
                if (underlyingSlotTemplate != value)
                {
                    BuildingSlotTemplate oldValue = underlyingSlotTemplate;
                    underlyingSlotTemplate = value;
                    UnderlyingSlotTemplateChanged?.Invoke(this, new ValueChangedEventArgs<BuildingSlotTemplate>(oldValue, value));
                }
            }
        }
        #endregion

        #region Constructors
        public BuildingSlot(BuildingSlotTemplate template, MapSpace parentSpace, BuildingSlotTemplate underlyingSlotTemplate)
        {
            Template = template;
            if (parentSpace == null)
            {
                throw new ArgumentNullException("BuildingSpace must have non-null parent map space");
            }
            ParentSpace = parentSpace;
            UnderlyingSlotTemplate = underlyingSlotTemplate;
        }
        #endregion

        #region Methods
        #endregion
    }
}
