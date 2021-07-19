using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.MetaComponents.TurnLogics
{
    public class StandardTurnLogic : TurnLogic
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public override void ExecuteGameTurn(GameInstance instance)
        {
            // Check for pop job promotions @TODO
            PromotePops(instance);
            // Assign pops to empty jobs @TODO
            // Work jobs in order of priority, low to high
            WorkJobs(instance);
            // Trade resources and fundamentals between pops @TODO
            // Advance constructions @TODO
            // Start new constructions @TODO
            // Check for pop growth @TODO
            // Check for pop migration @TODO
        }

        protected void PromotePops(GameInstance instance)
        {
            List<Job> emptyJobs = instance.AllJobs.Where(job => job.Worker != null).ToList();
            while (emptyJobs.Count > 0)
            {
                Job emptyJob = emptyJobs[0];
                emptyJobs.RemoveAt(0);
                // Find all pops in the same space as the empty job and order them by current pay, high to low
                List<Pop> popsInSpaceByPay = instance.AllPops.Where(pop => pop.Space == emptyJob.Space).OrderByDescending(pop => { return pop.Job == null ? 0 : GetEstimatedNetPay(pop.Job); }).ToList();
                // Filter out pops whose base pay is already equal to or above the empty job's
                // @TODO: improve performance by doing only one lookup
                popsInSpaceByPay = popsInSpaceByPay.Where(pop => pop.Job == null ? true : GetEstimatedNetPay(pop.Job) < GetEstimatedNetPay(emptyJob)).ToList();
                Pop targetPop = popsInSpaceByPay.Count > 0 ? popsInSpaceByPay.First() : null;
                if (targetPop != null)
                {
                    Job oldJob = targetPop.Job;
                    targetPop.Job = emptyJob;
                    emptyJob.Worker = targetPop;
                    oldJob.Worker = null;
                    emptyJobs.Insert(0, oldJob);
                }
            }
        }

        protected void WorkJobs(GameInstance instance)
        {
            // Order pops by job priority
            List<Pop>[] popsByJobPriority = new List<Pop>[JobTemplate.MAX_JOB_PRIORITY];
            for (int i = 0; i < popsByJobPriority.Length; i++)
            {
                popsByJobPriority[i] = new List<Pop>();
            }
            foreach (Pop pop in instance.AllPops)
            {
                if (pop.Job != null)
                {
                    popsByJobPriority[pop.Job.Template.Priority].Add(pop);
                }
                else
                {
                    popsByJobPriority[JobTemplate.UNEMPLOYED_JOB_PRIORITY].Add(pop);
                }
            }
            // Work each pop's job
            for (int i = 0; i < popsByJobPriority.Length; i++)
            {
                foreach (Pop pop in popsByJobPriority[i])
                {
                    if (pop.Job != null && WorkJob(pop.Job, pop))
                    {

                    }
                    else
                    {
                        HandleUnemployedPop(pop);
                    }
                }
            }
        }

        /// <summary>
        /// Works the given Job with the given worker Pop, consuming inputs from their resources and providing to them the outputs
        /// </summary>
        /// <param name="job">The Job to work</param>
        /// <param name="workerPop">The Pop to work the given Job</param>
        /// <returns>True if the Job was successfully worked. False otherwise</returns>
        protected bool WorkJob(Job job, Pop workerPop)
        {
            if (job.Template.Inputs.IsSatisfiedBy(workerPop.OwnedResources))
            {
                workerPop.OwnedResources.Subtract(job.Template.Inputs);
                workerPop.OwnedResources.Add(job.Template.Outputs);
                workerPop.Money += job.Template.BasePay;
                return true;
            }
            return false;
        }

        protected void HandleUnemployedPop(Pop pop)
        {
            // Remove pop from job if they have one but could not provide needed inputs
            if (pop.Job != null)
            {
                pop.Job.Worker = null;
                pop.Job = null;
            }
            // @TODO
        }

        protected decimal GetEstimatedNetPay(Job job)
        {
            return job.Template.BasePay + job.Template.Outputs.BaseValue - job.Template.Inputs.BaseValue;
        }
        #endregion
    }
}
