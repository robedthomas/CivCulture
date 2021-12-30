﻿using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class JobViewModel : BaseViewModel
    {
        #region Events
        #endregion

        #region Fields
        private Job sourceJob;
        private PopViewModel popVM;
        #endregion

        #region Properties
        public Job SourceJob
        {
            get => sourceJob;
            set
            {
                if (sourceJob != value)
                {
                    if (sourceJob != null)
                    {
                        sourceJob.WorkerChanged -= SourceJob_WorkerChanged;
                    }
                    sourceJob = value;
                    if (sourceJob != null)
                    {
                        sourceJob.WorkerChanged += SourceJob_WorkerChanged;
                    }
                    if (sourceJob == null || sourceJob.Worker == null)
                    {
                        PopVM = null;
                    }
                    else
                    {
                        PopVM = new PopViewModel() { SourcePop = sourceJob.Worker };
                    }
                    OnPropertyChanged();
                }
            }
        }

        public PopViewModel PopVM
        {
            get => popVM;
            set
            {
                if (popVM != value)
                {
                    popVM = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public JobViewModel(Job sourceJob)
        {
            SourceJob = sourceJob;
        }
        #endregion

        #region Methods
        private void SourceJob_WorkerChanged(object sender, CivCulture_Model.Events.ValueChangedEventArgs<Pop> e)
        {
            if (e.NewValue == null)
            {
                PopVM = null;
            }
            else
            {
                PopVM = new PopViewModel() { SourcePop = e.NewValue };
            }
        }
        #endregion
    }
}
