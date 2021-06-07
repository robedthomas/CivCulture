using CivCulture_Model.Events;
using System;
using System.Collections.Generic;
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
                    SpacesChanged?.Invoke(this, new ValueChangedEventArgs<MapSpaceCollection>(spaces, value));
                    spaces = value;
                    if (spaces != null)
                    {
                        spaces.CollectionChanged += Spaces_CollectionChanged;
                    }
                }
            }
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        private void Spaces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MapSpace newSpace in e.NewItems)
                {
                    // @TODO
                }
            }
            if (e.OldItems != null)
            {
                foreach (MapSpace oldSpace in e.OldItems)
                {
                    // @TODO
                }
            }
        }
        #endregion
    }
}
