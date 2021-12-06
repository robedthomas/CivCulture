using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using CivCulture_Model.Utilities;
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
        private const decimal POP_SATISFACTION_INCREASE = 0.1M;
        private const decimal POP_SATISFACTION_DECREASE = -0.2M;
        private const decimal POP_GROWTH_FACTOR = 0.2M;
        private const decimal POP_MIGRATION_SATISFACTION_THRESHOLD = 0.25M;
        private const decimal POP_MIGRATION_SATISFACTION_CHANGE = 0.1M;
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
            // Clear out all forecasts
            ClearForecasts(instance);
            // Check for pop job promotions and assign pops to empty job
            PromotePops(instance);
            // Work jobs in order of priority, low to high
            WorkJobs(instance);
            // Trade resources and fundamentals between pops @TODO
            // Advance constructions @TODO
            // Start new constructions @TODO
            // Consume pops' needs @TODO
            ConsumePopNeeds(instance);
            // Check for pop growth
            GrowPops(instance);
            // Check for pop migration @TODO
            MigratePops(instance);
        }

        protected void ClearForecasts(GameInstance instance)
        {
            // Clear out pop forecasts
            foreach (Pop pop in instance.AllPops)
            {
                pop.Forecast.MoneyChange.Modifiers.Clear();
                pop.Forecast.SatisfactionChange.Modifiers.Clear();
            }
            // @TODO: Clear out space forecasts
        }

        protected void PromotePops(GameInstance instance)
        {
            List<Job> emptyJobs = instance.AllJobs.Where(job => job.Worker == null).ToList();
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
            List<Pop>[] popsByJobPriority = new List<Pop>[JobTemplate.MAX_JOB_PRIORITY + 1];
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
                workerPop.Forecast.MoneyChange.Modifiers.Add(new Modifier<decimal>("Job Base Pay", job.Template.BasePay));
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

        protected void ConsumePopNeeds(GameInstance instance)
        {
            foreach (Pop pop in instance.AllPops)
            {
                // For each need type, consume those resources if the Pop deems it worth it
                if (pop.Template.Needs.TryGet(NeedType.Necessity, out ConsumeablesCollection necessities))
                {
                    if (necessities.IsSatisfiedBy(pop.OwnedResources))
                    {
                        pop.OwnedResources.Subtract(necessities);
                        pop.Satisfaction += POP_SATISFACTION_INCREASE;
                        pop.Forecast.SatisfactionChange.Modifiers.Add(new Modifier<decimal>("Necessities Met", POP_SATISFACTION_INCREASE));
                    }
                    else
                    {
                        // Failed to satisfy necessities @TODO
                        pop.Satisfaction += POP_SATISFACTION_DECREASE;
                        pop.Forecast.SatisfactionChange.Modifiers.Add(new Modifier<decimal>("Necessities Not Met", POP_SATISFACTION_DECREASE));
                        // @TODO: subtract as many resources as possible
                    }
                }
                // Do same for comforts and luxuries
            }
        }

        protected decimal GetEstimatedNetPay(Job job)
        {
            return job.Template.BasePay + job.Template.Outputs.BaseValue - job.Template.Inputs.BaseValue;
        }

        protected void GrowPops (GameInstance instance)
        {
            List<Pop> newPops = new List<Pop>();
            foreach (Pop pop in instance.AllPops)
            {
                pop.Space.PopGrowthProgress += pop.Satisfaction * POP_GROWTH_FACTOR;
                while (pop.Space.PopGrowthProgress >= 1M)
                {
                    pop.Space.PopGrowthProgress--;
                    Pop newPop = new Pop(pop.Space.NextPopTemplate) { Space = pop.Space };
                    pop.Space.NextPopTemplate = GetNextPopTemplate(pop.Space);
                    newPops.Add(newPop);
                }
            }
            foreach (Pop newPop in newPops)
            {
                instance.AllPops.Add(newPop);
            }
        }

        protected void MigratePops(GameInstance instance)
        {
            foreach (Pop pop in instance.AllPops)
            {
                // Unemployed or very dissatisfied pops migrate
                if (pop.Job == null || pop.Satisfaction <= POP_MIGRATION_SATISFACTION_THRESHOLD)
                {
                    MigratePop(pop, instance);
                }
            }
        }

        protected void MigratePop(Pop pop, GameInstance instance)
        {
            List<MapSpace> adjacentSpaces = instance.Map.Spaces.GetAllSpacesWithinDistance(pop.Space, 1, false);
            // Check adjacent spaces for free jobs
            List<MapSpace> possibleDestinations = adjacentSpaces.Where(space => space.Jobs.Any(job => job.Worker == null)).ToList();
            // If no free jobs, choose an adjacent space at random
            if (possibleDestinations.Count == 0)
            {
                possibleDestinations = adjacentSpaces;
            }
            if (possibleDestinations.Count > 0)
            {
                MigratePop(pop, possibleDestinations.PickRandom(instance.RandomSeed), instance);
            }
        }

        protected void MigratePop(Pop pop, MapSpace destination, GameInstance instance)
        {
            pop.Space = destination;
            pop.Satisfaction += POP_MIGRATION_SATISFACTION_CHANGE;
            pop.Forecast.SatisfactionChange.Modifiers.Add(new Modifier<decimal>("Migration", POP_MIGRATION_SATISFACTION_CHANGE));
        }

        protected PopTemplate GetNextPopTemplate(MapSpace space)
        {
            return PopTemplate.HunterGatherer;
        }

        public override void InitGameInstance(GameInstance instance)
        {
            foreach (MapSpace space in instance.Map.Spaces)
            {
                space.NextPopTemplate = GetNextPopTemplate(space);
            }
        }
        #endregion
    }
}
