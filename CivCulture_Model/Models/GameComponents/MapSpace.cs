using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class MapSpace : GameComponent
    {
        #region Fields
        private Terrain terrain;
        #endregion

        #region Events
        public event ValueChangedEventHandler<Terrain> TerrainChanged;
        #endregion

        #region Properties
        public int Row { get; protected set; }

        public int Column { get; protected set; }

        public string Name { get; set; }

        public Terrain Terrain
        {
            get => terrain;
            set
            {
                if (terrain != value)
                {
                    Terrain oldValue = terrain;
                    terrain = value;
                    TerrainChanged?.Invoke(this, new ValueChangedEventArgs<Terrain>(oldValue, terrain));
                }
            }
        }

        public ObservableCollection<Pop> Pops { get; protected set; }

        public ObservableCollection<Job> Jobs { get; protected set; }

        public ObservableCollection<TerrainResource> TerrainResources { get; protected set; }
        #endregion

        #region Constructors
        public MapSpace(int row, int column, Terrain terrain, params TerrainResource[] terrainResources)
        {
            Row = row;
            Column = column;
            Terrain = terrain;
            Pops = new ObservableCollection<Pop>();
            Jobs = new ObservableCollection<Job>();
            TerrainResources = new ObservableCollection<TerrainResource>(terrainResources);
        }
        #endregion

        #region Methods
        #endregion
    }
}
