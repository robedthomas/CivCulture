using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.MetaComponents;
using CivCulture_Model.Models.MetaComponents.UserMutables;
using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class GameInstance : BaseModel
    {
        #region Fields
        public const string DEFAULT_NAMES_DATABASE_LOCATION = "NamesDatabase.xml";
        public const string DEFAULT_TEMPLATES_DATABASE_LOCATION = "TemplateDefinitions.xml";

        private GameMap map;
        private ObservableCollection<Culture> allCultures;
        private ObservableCollection<Job> allJobs;
        private ObservableCollection<Pop> allPops;
        private MapGeneration mapGeneration;
        private MapConfiguration mapConfig;
        private TurnLogic turnLogic;
        private Random randomSeed;
        #endregion

        #region Events
        public event ValueChangedEventHandler<GameMap> MapChanged;
        public event ValueChangedEventHandler<ObservableCollection<Culture>> AllCulturesChanged;
        public event ValueChangedEventHandler<ObservableCollection<Job>> AllJobsChanged;
        public event ValueChangedEventHandler<ObservableCollection<Pop>> AllPopsChanged;
        public event ValueChangedEventHandler<MapGeneration> MapGenerationChanged;
        public event ValueChangedEventHandler<MapConfiguration> MapConfigChanged;
        public event ValueChangedEventHandler<TurnLogic> TurnLogicChanged;
        public event ValueChangedEventHandler<Random> RandomSeedChanged;
        #endregion

        #region Properties
        public GameMap Map
        {
            get => map;
            set
            {
                if (map != value)
                {
                    GameMap oldValue = map;
                    map = value;
                    MapChanged?.Invoke(this, new ValueChangedEventArgs<GameMap>(oldValue, map));
                }
            }
        }

        public ObservableCollection<Culture> AllCultures
        {
            get => allCultures;
            protected set
            {
                if (allCultures != value)
                {
                    ObservableCollection<Culture> oldValue = allCultures;
                    allCultures = value;
                    AllCulturesChanged?.Invoke(this, new ValueChangedEventArgs<ObservableCollection<Culture>>(oldValue, allCultures));
                }
            }
        }

        public ObservableCollection<Job> AllJobs
        {
            get => allJobs;
            protected set
            {
                if (allJobs != value)
                {
                    ObservableCollection<Job> oldValue = allJobs;
                    allJobs = value;
                    AllJobsChanged?.Invoke(this, new ValueChangedEventArgs<ObservableCollection<Job>>(oldValue, allJobs));
                }
            }
        }

        public ObservableCollection<Pop> AllPops
        {
            get => allPops;
            protected set
            {
                if (allPops != value)
                {
                    ObservableCollection<Pop> oldValue = allPops;
                    allPops = value;
                    AllPopsChanged?.Invoke(this, new ValueChangedEventArgs<ObservableCollection<Pop>>(oldValue, allPops));
                }
            }
        }

        public ObservableCollection<TechnologyTemplate> AllTechs { get; protected set; }

        public MapGeneration MapGeneration
        {
            get => mapGeneration;
            set
            {
                if (mapGeneration != value)
                {
                    MapGeneration oldValue = mapGeneration;
                    mapGeneration = value;
                    MapGenerationChanged?.Invoke(this, new ValueChangedEventArgs<MapGeneration>(oldValue, mapGeneration));
                }
            }
        }

        public MapConfiguration MapConfig
        {
            get => mapConfig;
            set
            {
                if (mapConfig != value)
                {
                    MapConfiguration oldValue = mapConfig;
                    mapConfig = value;
                    MapConfigChanged?.Invoke(this, new ValueChangedEventArgs<MapConfiguration>(oldValue, mapConfig));
                }
            }
        }

        public TurnLogic TurnLogic
        {
            get => turnLogic;
            set
            {
                if (turnLogic != value)
                {
                    TurnLogic oldValue = turnLogic;
                    turnLogic = value;
                    TurnLogicChanged?.Invoke(this, new ValueChangedEventArgs<TurnLogic>(oldValue, turnLogic));
                }
            }
        }

        public Random RandomSeed
        {
            get => randomSeed;
            set
            {
                if (randomSeed != value)
                {
                    Random oldValue = randomSeed;
                    randomSeed = value;
                    RandomSeedChanged?.Invoke(this, new ValueChangedEventArgs<Random>(oldValue, value));
                }
            }
        }

        public NamesDatabase NamesDB { get; protected set; }

        public TemplatesDatabase TemplatesDB { get; protected set; }
        #endregion

        #region Constructors
        public GameInstance()
        {
            RandomSeed = new Random();
            NamesDB = new NamesDatabase(DEFAULT_NAMES_DATABASE_LOCATION);
            TemplatesDB = new TemplatesDatabase(DEFAULT_TEMPLATES_DATABASE_LOCATION);
            AllTechs = GetStandardTechs(TemplatesDB);
        }

        public GameInstance(int seed) : this()
        {
            RandomSeed = new Random(seed);
        }
        #endregion

        #region Methods
        public void GenerateMap()
        {
            List<Culture> cultures;
            List<Pop> pops;
            List<Job> jobs;
            Map = MapGeneration.GenerateMap(MapConfig, NamesDB, TemplatesDB, AllTechs, RandomSeed, out cultures, out pops, out jobs);
            AllCultures = new ObservableCollection<Culture>(cultures);
            AllPops = new ObservableCollection<Pop>(pops);
            AllJobs = new ObservableCollection<Job>(jobs);
        }

        public void PassTurn()
        {
            TurnLogic.ExecuteGameTurn(this, NamesDB);
        }

        private ObservableCollection<TechnologyTemplate> GetStandardTechs(TemplatesDatabase templates)
        {
            ObservableCollection<TechnologyTemplate> techs = new ObservableCollection<TechnologyTemplate>();

            TechnologyTemplate permanentSettlement1 = new TechnologyTemplate("Permanent Settlement (I)", TechnologyCategory.Infrastructure, new ConsumeablesCollection() { { Fundamental.Progress, 5 } });
            permanentSettlement1.Modifiers.Add(StatModification.CultureEnableBuilding, TemplatesDB.GetBuildingTemplate("Mud Huts"), null, null);
            permanentSettlement1.Modifiers.Add(StatModification.SpaceJobs, templates.GetJobTemplate("Builder"), null, new TechModifier<decimal>(permanentSettlement1, 1));
            techs.Add(permanentSettlement1);

            //TechnologyTemplate permanentSettlement2 = new TechnologyTemplate("Permanent Settlement (II)", new ConsumeablesCollection() { { Fundamental.Progress, 5 } });
            //permanentSettlement2.Modifiers.Add(StatModification.)

            TechnologyTemplate agriculture1 = new TechnologyTemplate("Agriculture (I)", TechnologyCategory.Agricultural, new ConsumeablesCollection() { { Fundamental.Progress, 10 } });
            agriculture1.Modifiers.Add(StatModification.CultureEnableBuilding, TemplatesDB.GetBuildingTemplate("PrimitiveFarm_Grassland"), null, null);
            agriculture1.Parents.Add(permanentSettlement1);
            permanentSettlement1.Children.Add(agriculture1);
            techs.Add(agriculture1);

            TechnologyTemplate organizedLabor1 = new TechnologyTemplate("Organized Labor (I)", TechnologyCategory.Infrastructure, new ConsumeablesCollection() { { Fundamental.Progress, 10 } });
            organizedLabor1.Modifiers.Add(StatModification.JobOutputs, templates.GetJobTemplate("Builder"), Fundamental.Production, new TechModifier<decimal>(organizedLabor1, 0.5M));
            organizedLabor1.Parents.Add(permanentSettlement1);
            permanentSettlement1.Children.Add(organizedLabor1);
            techs.Add(organizedLabor1);

            TechnologyTemplate elderReverence1 = new TechnologyTemplate("Elder Reverence (I)", TechnologyCategory.Cultural, new ConsumeablesCollection() { { Fundamental.Progress, 5 } });
            elderReverence1.Modifiers.Add(StatModification.SpaceJobs, templates.GetJobTemplate("Elder"), null, new TechModifier<decimal>(elderReverence1, 1));
            techs.Add(elderReverence1);

            TechnologyTemplate elderReverence2 = new TechnologyTemplate("Elder Reverence (II)", TechnologyCategory.Cultural, new ConsumeablesCollection() { { Fundamental.Progress, 10 } });
            elderReverence2.Modifiers.Add(StatModification.JobInputs, templates.GetJobTemplate("Elder"), Fundamental.Food, new TechModifier<decimal>(elderReverence2, 0.5M));
            elderReverence2.Modifiers.Add(StatModification.JobOutputs, templates.GetJobTemplate("Elder"), Fundamental.Progress, new TechModifier<decimal>(elderReverence2, 0.5M));
            elderReverence2.Parents.Add(elderReverence1);
            elderReverence1.Children.Add(elderReverence2);
            techs.Add(elderReverence2);

            return techs;
        }
        #endregion
    }
}
