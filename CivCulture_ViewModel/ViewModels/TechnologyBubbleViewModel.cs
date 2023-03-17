using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class TechnologyBubbleViewModel : BaseViewModel
    {
        #region Fields
        private Technology sourceTech;
        #endregion

        #region Properties
        public Technology SourceTech
        {
            get => sourceTech;
            set
            {
                if (sourceTech != value)
                {
                    if (sourceTech != null)
                    {
                        sourceTech.CompletionLevelChanged -= SourceTech_CompletionLevelChanged;
                        sourceTech.Owner.CurrentResearchChanged -= OwnerCulture_CurrentResearchChanged;
                        sourceTech.Owner.AvailableTechnologies.CollectionChanged -= OwnerCulture_AvailableTechnologies_CollectionChanged;
                    }
                    sourceTech = value;
                    if (sourceTech != null)
                    {
                        sourceTech.CompletionLevelChanged += SourceTech_CompletionLevelChanged;
                        sourceTech.Owner.CurrentResearchChanged += OwnerCulture_CurrentResearchChanged;
                        sourceTech.Owner.AvailableTechnologies.CollectionChanged += OwnerCulture_AvailableTechnologies_CollectionChanged;
                    }
                    OnPropertyChanged(nameof(SourceTech));
                    OnPropertyChanged(nameof(ResearchState));
                    OnPropertyChanged(nameof(TechCategory));
                }
            }
        }

        public ResearchState ResearchState
        {
            get
            {
                if (SourceTech.IsComplete)
                {
                    return ResearchState.Researched;
                }
                else if (SourceTech == SourceTech.Owner.CurrentResearch)
                {
                    return ResearchState.BeingResearched;
                }
                else if (SourceTech.Owner.AvailableTechnologies.Contains(SourceTech))
                {
                    return ResearchState.AvailableForResearch;
                }
                else
                {
                    return ResearchState.UnavailableForResearch;
                }
                // @TODO: Add queueing up techs for research
            }
        }

        public TechnologyCategory TechCategory
        {
            get => SourceTech.Template.Category;
        }
        #endregion

        #region Constructors
        public TechnologyBubbleViewModel(Technology sourceTech)
        {
            SourceTech = sourceTech;
            
        }

        private void SourceTech_CompletionLevelChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<decimal> e)
        {
            OnPropertyChanged(nameof(ResearchState));
        }

        private void OwnerCulture_AvailableTechnologies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ResearchState));
        }

        private void OwnerCulture_CurrentResearchChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<Technology> e)
        {
            OnPropertyChanged(nameof(ResearchState));
        }
        #endregion

        #region Methods
        #endregion
    }
}
