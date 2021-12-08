using CivCulture_Model.Events;
using CivCulture_Model.Models.MetaComponents;
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
        private GameMap map;
        private ObservableCollection<Job> allJobs;
        private ObservableCollection<Pop> allPops;
        private MapGeneration mapGeneration;
        private MapConfiguration mapConfig;
        private TurnLogic turnLogic;
        private Random randomSeed;
        #endregion

        #region Events
        public event ValueChangedEventHandler<GameMap> MapChanged;
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
        #endregion

        #region Constructors
        public GameInstance()
        {
            RandomSeed = new Random();
        }

        public GameInstance(int seed)
        {
            RandomSeed = new Random(seed);
        }
        #endregion

        #region Methods
        public void GenerateMap()
        {
            List<Pop> pops;
            List<Job> jobs;
            Map = MapGeneration.GenerateMap(MapConfig, RandomSeed, out pops, out jobs);
            AllPops = new ObservableCollection<Pop>(pops);
            AllJobs = new ObservableCollection<Job>(jobs);
        }

        public void PassTurn()
        {
            TurnLogic.ExecuteGameTurn(this);
        }
        #endregion
    }
}
