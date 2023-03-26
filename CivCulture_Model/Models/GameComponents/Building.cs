using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    [DebuggerDisplay("{Template.Name} Building")]
    public class Building : JobSource, ITemplated<BuildingTemplate>, IFulfillable<ConsumeablesCollection>
    {
        #region Events
        public event ValueChangedEventHandler<BuildingTemplate> TemplateChanged;
        public event ValueChangedEventHandler<MapSpace> SpaceChanged;
        public event ValueChangedEventHandler<BuildingSlot> ParentSlotChanged;
        public event ValueChangedEventHandler<ConsumeablesCollection> RemainingCostsChanged;
        public event ValueChangedEventHandler<decimal> CompletionLevelChanged;
        #endregion

        #region Fields
        private BuildingTemplate template;
        private MapSpace space;
        private BuildingSlot parentSlot;
        private ConsumeablesCollection remainingCosts;
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

        public BuildingSlot ParentSlot
        {
            get => parentSlot;
            set
            {
                if (parentSlot != value)
                {
                    BuildingSlot oldValue = parentSlot;
                    parentSlot = value;
                    ParentSlotChanged?.Invoke(this, new ValueChangedEventArgs<BuildingSlot>(oldValue, value));
                }
            }
        }

        public ConsumeablesCollection TotalCosts
        {
            get => Template.Costs;
        }

        public ConsumeablesCollection RemainingCosts
        {
            get => remainingCosts;
            protected set
            {
                if (remainingCosts != value)
                {
                    ConsumeablesCollection oldValue = remainingCosts;
                    remainingCosts = value;
                    RemainingCostsChanged?.Invoke(this, new ValueChangedEventArgs<ConsumeablesCollection>(oldValue, value));
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
        public Building(BuildingTemplate template, MapSpace space, BuildingSlot parentSlot)
        {
            Template = template;
            Space = space;
            ParentSlot = parentSlot;
            CompletionLevel = 0;
            RemainingCosts = new ConsumeablesCollection(TotalCosts);
            foreach (JobTemplate childJobTemplate in Template.Jobs)
            {
                ChildJobs.Add(new Job(childJobTemplate, this));
            }
        }
        #endregion

        #region Methods
        public static decimal GetCompletionLevel(Building targetBuilding)
        {
            decimal originalCostCount = targetBuilding.TotalCosts.Values.Sum();
            decimal remainingCostCount = targetBuilding.RemainingCosts.Values.Sum();
            return 1M - (remainingCostCount / originalCostCount);
        }
        #endregion
    }
}
