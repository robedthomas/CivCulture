using GenericUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CivCulture_Model.Models.Collections
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
            set
            {
                MapSpace oldValue = this[row, col];
                allSpaces[row, col] = value;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this[row, col], oldValue));
            }
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
                throw new ArgumentOutOfRangeException("MapSpaceCollection: width must be positive");
            }
            if (height > 0)
            {
                Height = height;
            }
            else
            {
                throw new ArgumentOutOfRangeException("MapSpaceCollection: height must be positive");
            }
            allSpaces = new MapSpace[height, width];
        }

        public MapSpaceCollection(int width, int height, IEnumerable<MapSpace> spaces) : this(width, height)
        {
            int row = 0, col = 0;
            foreach (MapSpace nextSpace in spaces)
            {
                this[row, col] = nextSpace;
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
        public static int GetDistance(MapSpace firstSpace, MapSpace secondSpace, bool includeDiagonals)
        {
            if (includeDiagonals)
            {
                return Math.Max(Math.Abs(firstSpace.Row - secondSpace.Row), Math.Abs(firstSpace.Column - secondSpace.Column));
            }
            else
            {
                return Math.Abs(firstSpace.Row - secondSpace.Row) + Math.Abs(firstSpace.Column - secondSpace.Column);
            }
        }

        public List<MapSpace> GetAllSpacesWithinDistance(MapSpace targetSpace, int distance, bool includeDiagonals)
        {
            if (allSpaces.TryFindIndex(targetSpace, out int targetCol, out int targetRow))
            {
                if (distance >= 0)
                {
                    List<MapSpace> output = new List<MapSpace>();
                    for (int i = 0; i <= allSpaces.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j <= allSpaces.GetUpperBound(1); j++)
                        {
                            if (allSpaces[i, j] != targetSpace && GetDistance(targetSpace, allSpaces[i, j], includeDiagonals) <= distance)
                            {
                                output.Add(allSpaces[i, j]);
                            }
                        }
                    }
                    return output;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Require non-negative distance, got {distance}");
                }
            }
            else
            {
                throw new ArgumentException($"Received invalid MapSpace as parameter to GetAllSpacesWithinDistance()");
            }
        }

        public MapSpace GetClosestSpace(MapSpace sourceSpace, IEnumerable<MapSpace> possibleSpaces, bool includeDiagonals)
        {
            return possibleSpaces.Aggregate((currentClosest, nextSpace) =>
            {
                if (GetDistance(sourceSpace, nextSpace, includeDiagonals) < GetDistance(sourceSpace, currentClosest, includeDiagonals)) // @TODO: improve performance by calling GetDistance() only once per space
                {
                    return nextSpace;
                }
                return currentClosest; // @NOTE: in ties, the first space checked wins
            });
        }

        #region IEnumerable<>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return allSpaces.GetEnumerator();
        }

        public IEnumerator<MapSpace> GetEnumerator()
        {
            return new MapSpaceCollectionEnumerator(allSpaces.Flatten2DArray());
        }
        #endregion
        #endregion
    }

    class MapSpaceCollectionEnumerator : IEnumerator<MapSpace>
    {
        private MapSpace[] spaces;
        private int index = -1;

        public MapSpaceCollectionEnumerator(MapSpace[] _spaces)
        {
            spaces = _spaces;
        }

        #region IEnumerator<>
        public MapSpace Current => spaces[index];

        object IEnumerator.Current => spaces[index];

        public void Dispose()
        {
            spaces = null;
            index = -1;
        }

        public bool MoveNext()
        {
            return ++index < spaces.Length;
        }

        public void Reset()
        {
            index = -1;
        }
        #endregion
    }
}
