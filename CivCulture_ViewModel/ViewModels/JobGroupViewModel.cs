using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class JobGroupViewModel : BaseViewModel
    {
        #region Events
        #endregion

        #region Fields
        private JobTemplate template;
        private ObservableCollection<JobViewModel> jobVMs;
        #endregion

        #region Properties
        public JobTemplate Template
        {
            get => template;
            set
            {
                if (template != value)
                {
                    template = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(JobName));
                }
            }
        }

        public string JobName
        {
            get => template != null ? template.DisplayName : string.Empty;
        }

        public ObservableCollection<JobViewModel> Jobs
        {
            get => jobVMs;
            set
            {
                if (jobVMs != value)
                {
                    jobVMs = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        private JobGroupViewModel()
        {
            Jobs = new ObservableCollection<JobViewModel>();
        }

        public JobGroupViewModel(JobTemplate template) : this()
        {
            Template = template;
        }
        #endregion

        #region Methods
        #endregion
    }
}
