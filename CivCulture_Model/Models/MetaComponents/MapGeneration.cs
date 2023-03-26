using CivCulture_Model.Models.MetaComponents.UserMutables;
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
        /// <param name="namesDb">All cultural names that may be used for culture generation</param>
        /// <param name="templatesDb">Database of all game components configurable by the user</param>
        /// <param name="allTechs">All technologies that may be researched during the game</param>
        /// <param name="seed">Random number generator to use for generation</param>
        /// <param name="allCultures">All Cultures generated during map generation</param>
        /// <param name="allPops">All Pops generated during map generation</param>
        /// <param name="allJobs">All Jobs generated during map generation</param>
        /// <returns></returns>
        public abstract GameMap GenerateMap(MapConfiguration config, NamesDatabase namesDb, TemplatesDatabase templatesDb, IEnumerable<TechnologyTemplate> allTechs, Random seed, out List<Culture> allCultures, out List<Pop> allPops, out List<Job> allJobs);
    }
}
