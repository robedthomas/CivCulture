using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CivCulture_Model.Models
{
    public class MapSpaceCollection : GameComponent, IEnumerable<MapSpace>, INotifyCollectionChanged
    {
        #region Fields
        private MapSpace[,] allSpaces;
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Properties
        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public MapSpace this[int row, int col]
        {
            get => allSpaces[row, col];
            set => allSpaces[row, col] = value;
        }
        #endregion

        #region Constructors
        public MapSpaceCollection(int width, int height)
        {
            if (width > 0)
            {
                Width = width;
            }
            else
            {
                throw new ArgumentException("MapSpaceCollection: width must be positive");
            }
            if (height > 0)
            {
                Height = height;
            }
            else
            {
                throw new ArgumentException("MapSpaceCollection: height must be positive");
            }
            allSpaces = new MapSpace[width, height];
        }

        public MapSpaceCollection(int width, int height, IEnumerable<MapSpace> spaces) : this(width, height)
        {
            int row = 0, col = 0;

            foreach (MapSpace nextSpace in spaces)
            {
                allSpaces[row, col] = nextSpace;
                if (++col >= Width)
                {
                    col = 0;
                    if (++row >= Height)
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region Methods
        #region IEnumerable<>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return allSpaces.GetEnumerator();
        }

        public IEnumerator<MapSpace> GetEnumerator()
        {
            return (IEnumerator<MapSpace>)allSpaces.GetEnumerator();
        }
        #endregion
        #endregion
    }
}
