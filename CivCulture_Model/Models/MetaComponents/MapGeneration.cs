﻿using CivCulture_Model.Models.MetaComponents.UserMutables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models.MetaComponents
{
    public abstract class MapGeneration : MetaComponent
    {
        /// <summary>
        /// Generates a new GameMap from the given configuration
        /// </summary>
        /// <param name="config">Configuration parameters for map generation</param>
        /// <param name="seed">Random number generator to use for generation</param>
        /// <param name="allCultures">All Cultures generated during map generation</param>
        /// <param name="allPops">All Pops generated during map generation</param>
        /// <param name="allJobs">All Jobs generated during map generation</param>
        /// <returns></returns>
        public abstract GameMap GenerateMap(MapConfiguration config, NamesDatabase namesDb, Random seed, out List<Culture> allCultures, out List<Pop> allPops, out List<Job> allJobs);
    }
}
