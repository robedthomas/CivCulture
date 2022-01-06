﻿using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using GenericUtilities;
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
        private const decimal POP_SATISFACTION_DECREASE = -0.3M;
        private const decimal POP_GROWTH_FACTOR = 0.2M;
        private const decimal POP_MIGRATION_SATISFACTION_THRESHOLD = 0.25M;
        private const decimal POP_MIGRATION_SATISFACTION_CHANGE = 0.1M;
        private const decimal DEFAULT_STOCKPILE_MONEY = 100M;
        private const decimal STOCKPILE_PURCHASE_VALUE_MODIFIER = 1.05M;
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
            // Produce resources from buildings
            ProduceBuildingResources(instance);
            // Work jobs in order of priority, low to high
            WorkJobs(instance);
            // Trade resources and fundamentals between pops
            TradePopResourcesWithStockpile(instance);
            // Advance constructions
            AdvanceConstructions(instance);
            // Start new constructions
            StartNewConstructions(instance);
            // Consume pops' needs
            ConsumePopNeeds(instance);
            // Check for pop growth
            GrowPops(instance);
            // Check for pop migration
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
                    if (oldJob != null)
                    {
                        oldJob.Worker = null;
                        emptyJobs.Insert(0, oldJob);
                    }
                }
            }
        }

        protected void ProduceBuildingResources(GameInstance instance)
        {
            foreach (MapSpace space in instance.Map.Spaces)
            {
                foreach(Building building in space.Buildings)
                {
                    if (building.IsComplete)
                    {
                        space.OwnedResources.Add(building.Template.Outputs);
                    }
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

        protected void TradePopResourcesWithStockpile(GameInstance instance)
        {
            foreach (MapSpace space in instance.Map.Spaces)
            {
                // Sell all excess resources to the space's stockpile
                foreach (Pop resident in space.Pops)
                {
                    SellPopResourcesToStockpile(resident);
                }
                // Buy all needed resources from the space's stockpile
                foreach (Pop resident in space.Pops)
                {
                    BuyPopResourcesFromStockpile(resident);
                }
            }
        }

        protected ConsumeablesCollection SellPopResourcesToStockpile(Pop pop)
        {
            // Compile all needs for this pop into one collection
            ConsumeablesCollection allNeeds = new ConsumeablesCollection();
            allNeeds.Add(pop.Template.Needs);
            // Sell all resources that aren't needed
            ConsumeablesCollection resourcesToSell = new ConsumeablesCollection(pop.OwnedResources);
            resourcesToSell.Subtract(allNeeds);
            ConsumeablesCollection resourcesSold = SellResourcesToStockpile(resourcesToSell, pop, pop.Space);
            return resourcesSold;
        }

        protected ConsumeablesCollection SellResourcesToStockpile(ConsumeablesCollection resourcesToSell, Pop owner, MapSpace stockpileSpace)
        {
            ConsumeablesCollection resourcesSold = new ConsumeablesCollection();
            foreach (Consumeable resource in resourcesToSell.Keys)
            {
                if (stockpileSpace.Money > 0)
                {
                    if (resourcesToSell[resource] > 0)
                    {
                        decimal countToSell = Math.Min(resourcesToSell[resource], stockpileSpace.Money / GetResourceSaleValue(resource, stockpileSpace));
                        decimal saleValue = GetResourceSaleValue(resource, stockpileSpace) * countToSell;
                        if (owner.TrySell(resource, countToSell, saleValue, stockpileSpace))
                        {
                            owner.Forecast.MoneyChange.Modifiers.Add(new Modifier<decimal>($"Sold {countToSell} {resource.Name}", saleValue));
                            resourcesSold.Add(resource, countToSell);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return resourcesSold;
        }

        protected ConsumeablesCollection BuyPopResourcesFromStockpile(Pop pop)
        {
            // Compile all needs for this pop into one collection
            ConsumeablesCollection allNeeds = new ConsumeablesCollection();
            allNeeds.Add(pop.Template.Needs);
            // Buy all needs not yet owned
            ConsumeablesCollection resourcesNeeded = new ConsumeablesCollection(allNeeds);
            resourcesNeeded.Subtract(pop.OwnedResources);
            ConsumeablesCollection resourcesBought = BuyResourcesFromStockpile(resourcesNeeded, pop, pop.Space);
            return resourcesBought;
        }

        protected ConsumeablesCollection BuyResourcesFromStockpile(ConsumeablesCollection resourcesToBuy, Pop owner, MapSpace stockpileSpace)
        {
            ConsumeablesCollection resourcesBought = new ConsumeablesCollection();
            foreach (Consumeable resource in resourcesToBuy.Keys)
            {
                if (owner.Money > 0)
                {
                    if (resourcesToBuy[resource] > 0 && stockpileSpace.OwnedResources.ContainsKey(resource))
                    {
                        decimal countToBuy = Math.Min(Math.Min(resourcesToBuy[resource], stockpileSpace.OwnedResources[resource]), owner.Money / GetResourcePurchaseValue(resource, stockpileSpace));
                        decimal purchaseValue = GetResourcePurchaseValue(resource, stockpileSpace) * countToBuy;
                        if (stockpileSpace.TrySell(resource, countToBuy, purchaseValue, owner))
                        {
                            owner.Forecast.MoneyChange.Modifiers.Add(new Modifier<decimal>($"Bought {countToBuy} {resource.Name}", -purchaseValue));
                            resourcesBought.Add(resource, countToBuy);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return resourcesBought;
        }

        protected decimal GetResourceSaleValue(Consumeable resource, MapSpace stockpileSpace)
        {
            return resource.BaseValue;
        }

        protected decimal GetResourcePurchaseValue(Consumeable resource, MapSpace stockpileSpace)
        {
            return resource.BaseValue * STOCKPILE_PURCHASE_VALUE_MODIFIER;
        }

        protected void AdvanceConstructions(GameInstance instance)
        {
            foreach (MapSpace space in instance.Map.Spaces)
            {
                if (space.CurrentConstruction != null)
                {
                    ConsumeablesCollection resourcesConsumed = GetResourcesToAdvanceConstruction(space.CurrentConstruction, space.OwnedResources);
                    space.OwnedResources.Subtract(resourcesConsumed);
                    space.CurrentConstruction.RemainingCosts.Subtract(resourcesConsumed);
                    space.CurrentConstruction.CompletionLevel = Building.GetCompletionLevel(space.CurrentConstruction);
                    if (space.CurrentConstruction.IsComplete)
                    {
                        space.Buildings.Add(space.CurrentConstruction);
                        space.CurrentConstruction = null;
                    }
                }
            }
        }

        protected ConsumeablesCollection GetResourcesToAdvanceConstruction(Building construction, ConsumeablesCollection availableResources)
        {
            ConsumeablesCollection resourcesConsumed = new ConsumeablesCollection();
            foreach (Consumeable req in construction.Template.Costs.Keys)
            {
                if (availableResources.ContainsKey(req))
                {
                    decimal countConsumed = Math.Min(availableResources[req], construction.Template.Costs[req]);
                    resourcesConsumed.Add(req, countConsumed);
                }
            }
            return resourcesConsumed;
        }

        protected void StartNewConstructions(GameInstance instance)
        {
            foreach (MapSpace space in instance.Map.Spaces)
            {
                // Only start new constructions for spaces that don't have a current construction
                if (space.CurrentConstruction is null)
                {
                    space.CurrentConstruction = GenerateNewConstructionForSpace(space, instance);
                }
            }
        }

        protected Building GenerateNewConstructionForSpace(MapSpace space, GameInstance instance)
        {
            if (space.AvailableBuildings.Count > 0)
            {
                // For now, just select any of the available building templates for this space
                // @TODO: base selection off of space's resources in demand
                BuildingTemplate targetTemplate = space.AvailableBuildings.PickRandom(instance.RandomSeed);
                return new Building(targetTemplate, space);
            }
            return null;
        }

        protected void ConsumePopNeeds(GameInstance instance)
        {
            foreach (Pop pop in instance.AllPops)
            {
                // For each need type, consume those resources if the Pop deems it worth it
                if (pop.Template.Needs.TryGetValue(NeedType.Necessity, out ConsumeablesCollection necessities))
                {
                    decimal needsSatisfactionRatio = SatisfyNeedsWithResources(necessities, pop.OwnedResources);
                    if (needsSatisfactionRatio == 1M)
                    {
                        pop.Satisfaction += POP_SATISFACTION_INCREASE;
                        pop.Forecast.SatisfactionChange.Modifiers.Add(new Modifier<decimal>("Necessities Met", POP_SATISFACTION_INCREASE));
                    }
                    else
                    {
                        // Failed to satisfy necessities @TODO
                        pop.Satisfaction += POP_SATISFACTION_DECREASE * (1 - needsSatisfactionRatio);
                        pop.Forecast.SatisfactionChange.Modifiers.Add(new Modifier<decimal>("Necessities Not Met", POP_SATISFACTION_DECREASE));
                        // @TODO: subtract as many resources as possible
                    }
                }
                // Do same for comforts and luxuries
            }
        }

        protected decimal SatisfyNeedsWithResources(ConsumeablesCollection needs, ConsumeablesCollection resources)
        {
            decimal totalNeedsCount = 0, unsatisfiedNeedsCount = 0;
            foreach (Consumeable need in needs.Keys)
            {
                totalNeedsCount += needs[need];
                unsatisfiedNeedsCount += SatisfyNeedWithResources(need, needs[need], resources);
            }
            return 1M - (unsatisfiedNeedsCount / totalNeedsCount);
        }

        protected decimal SatisfyNeedWithResources(Consumeable need, decimal countNeeded, ConsumeablesCollection resources)
        {
            decimal countConsumed;
            if (resources.ContainsKey(need))
            {
                countConsumed = Math.Min(countNeeded, resources[need]);
            }
            else
            {
                countConsumed = 0;
            }
            resources.Subtract(need, countConsumed);
            return countNeeded - countConsumed;
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
            if (pop.Job != null)
            {
                pop.Job.Worker = null;
                pop.Job = null;
            }
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
                space.Money = DEFAULT_STOCKPILE_MONEY;
                space.Buildings.CollectionChanged += Space_Buildings_CollectionChanged;
                space.AvailableBuildings.Add(BuildingTemplate.MudHuts);
            }
        }

        private void Space_Buildings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Building newBuilding in e.NewItems)
                {
                    if (newBuilding.Template.IsSpaceUnique)
                    {
                        (sender as MapSpace).AvailableBuildings.Remove(newBuilding.Template);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (Building oldBuilding in e.OldItems)
                {
                    if (oldBuilding.Template.IsSpaceUnique)
                    {
                        (sender as MapSpace).AvailableBuildings.Add(oldBuilding.Template);
                    }
                }
            }
        }
        #endregion
    }
}
