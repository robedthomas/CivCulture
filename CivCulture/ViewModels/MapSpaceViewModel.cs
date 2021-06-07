using CivCulture_Model.Events;
using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CivCulture.ViewModels
{
    public class MapSpaceViewModel : BaseViewModel
    {
        #region Fields
        private MapSpace sourceSpace;
        #endregion

        #region Events
        #endregion

        #region Properties
        public MapSpace SourceSpace
        {
            get => sourceSpace;
            set
            {
                if (sourceSpace != value)
                {
                    if (sourceSpace != null)
                    {
                        sourceSpace.Pops.CollectionChanged -= Pops_CollectionChanged;
                        sourceSpace.Jobs.CollectionChanged -= Jobs_CollectionChanged;
                        sourceSpace.TerrainResources.CollectionChanged -= TerrainResources_CollectionChanged;
                        sourceSpace.TerrainChanged -= Terrain_Changed;
                    }
                    sourceSpace = value;
                    if (sourceSpace != null)
                    {
                        sourceSpace.Pops.CollectionChanged += Pops_CollectionChanged;
                        sourceSpace.Jobs.CollectionChanged += Jobs_CollectionChanged;
                        sourceSpace.TerrainResources.CollectionChanged += TerrainResources_CollectionChanged;
                        sourceSpace.TerrainChanged += Terrain_Changed;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PopCount));
                    OnPropertyChanged(nameof(BackgroundBrush));
                }
            }
        }

        public Brush BackgroundBrush
        {
            get
            {
                if (SourceSpace != null && SourceSpace.Terrain != null)
                {
                    return Brushes.Green; // @TODO
                }
                return Brushes.Black;
            }
        }

        public int PopCount
        {
            get { return SourceSpace.Pops.Count; }
        }
        #endregion

        #region Constructors
        public MapSpaceViewModel(MapSpace sourceSpace)
        {
            SourceSpace = sourceSpace;
        }
        #endregion

        #region Methods
        private void Pops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException(); // @TODO
        }

        private void Jobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException(); // @TODO
        }

        private void TerrainResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException(); // @TODO
        }

        private void Terrain_Changed(object sender, ValueChangedEventArgs<Terrain> e)
        {
            OnPropertyChanged(nameof(BackgroundBrush));
        }
        #endregion
    }
}
