using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class GameMap : GameComponent
    {
        #region Fields
        private MapSpaceCollection spaces;
        #endregion

        #region Events
        public event ValueChangedEventHandler<MapSpaceCollection> SpacesChanged;
        #endregion

        #region Properties
        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public MapSpaceCollection Spaces
        {
            get
            {
                return spaces;
            }
            set
            {
                if (spaces != value)
                {
                    if (spaces != null)
                    {
                        spaces.CollectionChanged -= Spaces_CollectionChanged;
                    }
                    MapSpaceCollection oldValue = spaces;
                    spaces = value;
                    if (spaces != null)
                    {
                        spaces.CollectionChanged += Spaces_CollectionChanged;
                    }
                    SpacesChanged?.Invoke(this, new ValueChangedEventArgs<MapSpaceCollection>(oldValue, spaces));
                }
            }
        }
        #endregion

        #region Constructors
        public GameMap(int width, int height)
        {
            Width = width;
            Height = height;
            Spaces = new MapSpaceCollection(width, height);
        }
        #endregion

        #region Methods
        private void Spaces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MapSpace newSpace in e.NewItems)
                {
                    newSpace.Pops.CollectionChanged += Spaces_Pops_CollectionChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (MapSpace oldSpace in e.OldItems)
                {
                    if (oldSpace != null)
                    {
                        oldSpace.Pops.CollectionChanged -= Spaces_Pops_CollectionChanged;
                    }
                }
            }
        }

        private void Spaces_Pops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Pop newPop in e.NewItems)
                {
                    
                }
            }
            if (e.OldItems != null)
            {
                foreach (Pop oldPop in e.OldItems)
                {
                    
                }
            }
        }
        #endregion
    }
}
