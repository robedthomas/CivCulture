using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System.Diagnostics;
using System.Linq;

namespace CivCulture_Model.Models
{
    public enum ResearchState
    {
        UnavailableForResearch,
        AvailableForResearch,
        BeingResearched,
        QueuedForResearch,
        Researched
    }

    public enum TechnologyCategory
    {
        Agricultural,
        Cultural,
        Scientific,
        Infrastructure,
        Mercantile,
        Military
    }

    [DebuggerDisplay("{Template.Name} Tech")]
    public class Technology : JobSource, ITemplated<TechnologyTemplate>, IFulfillable<ConsumeablesCollection>
    {
        #region Events
        public event ValueChangedEventHandler<decimal> CompletionLevelChanged;
        #endregion

        #region Fields
        private decimal completionLevel;
        #endregion

        #region Properties
        public TechnologyTemplate Template { get; protected set; }

        public Culture Owner { get; protected set; }

        public ConsumeablesCollection TotalCosts 
        { 
            get => Template.Costs;
        }

        public ConsumeablesCollection RemainingCosts { get; protected set; }

        public decimal CompletionLevel
        {
            get => completionLevel;
            protected set
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
        public Technology(TechnologyTemplate template, Culture owner)
        {
            Template = template;
            Owner = owner;
            RemainingCosts = new ConsumeablesCollection(Template.Costs);
            RemainingCosts.CollectionChanged += RemainingCosts_CollectionChanged;
        }
        #endregion

        #region Methods
        public static decimal GetCompletionLevel(Technology targetTech)
        {
            decimal originalCostCount = targetTech.TotalCosts.Values.Sum();
            decimal remainingCostCount = targetTech.RemainingCosts.Values.Sum();
            return 1M - (remainingCostCount / originalCostCount);
        }

        private void RemainingCosts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CompletionLevel = GetCompletionLevel(this);
        }
        #endregion
    }
}
