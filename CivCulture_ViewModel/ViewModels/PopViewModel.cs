using CivCulture_Model.Models;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CivCulture_ViewModel.ViewModels
{
    public class PopViewModel : BaseViewModel
    {
        #region Events
        #endregion

        #region Fields
        private Pop sourcePop;
        #endregion

        #region Properties
        public Pop SourcePop
        {
            get => sourcePop;
            set
            {
                if (sourcePop != value)
                {
                    if (sourcePop != null)
                    {
                        UnsubscribeFromSourcePopEvents();
                    }
                    sourcePop = value;
                    if (sourcePop != null)
                    {
                        SubscribeToSourcePopEvents();
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Money));
                    OnPropertyChanged(nameof(Job));
                    OnPropertyChanged(nameof(JobName));
                    OnPropertyChanged(nameof(OwnedResources));
                    OnPropertyChanged(nameof(Needs));
                }
            }
        }

        public decimal Money
        {
            get => SourcePop.Money;
        }

        public Modifiable<decimal> MoneyChange
        {
            get => SourcePop.Forecast.MoneyChange;
        }

        public decimal Satisfaction
        {
            get => SourcePop.Satisfaction;
        }

        public Modifiable<decimal> SatisfactionChange
        {
            get => SourcePop.Forecast.SatisfactionChange;
        }

        public Job Job
        {
            get => SourcePop.Job;
        }

        public string JobName
        {
            get => SourcePop.Job == null ? "Unemployed" : SourcePop.Job.Template.Name;
        }

        public ConsumeablesCollection OwnedResources
        {
            get => SourcePop.OwnedResources;
        }

        public NeedCollection Needs
        {
            get => SourcePop.Template == null ? null : SourcePop.Template.Needs;
        }

        public Brush FillBrush
        {
            get => Brushes.Green; // @TODO: link to current job
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        private void UnsubscribeFromSourcePopEvents()
        {
            SourcePop.TemplateChanged -= SourcePop_TemplateChanged;
            SourcePop.JobChanged -= SourcePop_JobChanged;
            SourcePop.MoneyChanged -= SourcePop_MoneyChanged;
            SourcePop.OwnedResourcesChanged -= SourcePop_OwnedResourcesChanged;
            SourcePop.SatisfactionChanged -= SourcePop_SatisfactionChanged;
        }

        private void SubscribeToSourcePopEvents()
        {
            SourcePop.TemplateChanged += SourcePop_TemplateChanged;
            SourcePop.JobChanged += SourcePop_JobChanged;
            SourcePop.MoneyChanged += SourcePop_MoneyChanged;
            SourcePop.OwnedResourcesChanged += SourcePop_OwnedResourcesChanged;
            SourcePop.SatisfactionChanged += SourcePop_SatisfactionChanged;
        }

        private void SourcePop_TemplateChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<PopTemplate> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.NeedsChanged -= SourcePop_Template_NeedsChanged;
            }
            if (e.NewValue != null)
            {
                e.NewValue.NeedsChanged += SourcePop_Template_NeedsChanged;
            }
        }

        private void SourcePop_Template_NeedsChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<CivCulture_Model.Models.Collections.NeedCollection> e)
        {
            OnPropertyChanged(nameof(Needs));
        }

        private void SourcePop_OwnedResourcesChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<ConsumeablesCollection> e)
        {
            OnPropertyChanged(nameof(OwnedResources));
        }

        private void SourcePop_MoneyChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<decimal> e)
        {
            OnPropertyChanged(nameof(Money));
        }

        private void SourcePop_JobChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<Job> e)
        {
            OnPropertyChanged(nameof(Job));
            OnPropertyChanged(nameof(JobName));
        }

        private void SourcePop_SatisfactionChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<decimal> e)
        {
            OnPropertyChanged(nameof(Satisfaction));
            OnPropertyChanged(nameof(SatisfactionChange));
        }
        #endregion
    }
}
