using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class MapSpaceCollection : GameComponent, IList<MapSpace>, INotifyCollectionChanged
    {
        protected List<MapSpace> allSpaces;

        public MapSpaceCollection()
        {
            allSpaces = new List<MapSpace>();
        }

        public MapSpaceCollection(IEnumerable<MapSpace> spaces)
        {
            allSpaces = new List<MapSpace>(spaces);
        }

        #region IList<>
        public MapSpace this[int index] 
        {
            get { return allSpaces[index]; }
            set { allSpaces[index] = value; }
        }

        public int Count { get { return allSpaces.Count; } }

        public bool IsReadOnly { get { return false; } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(MapSpace item)
        {
            allSpaces.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<MapSpace>() { item }, null));
        }

        public void Clear()
        {
            List<MapSpace> allRemovedItems = new List<MapSpace>(allSpaces);
            allSpaces.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, allRemovedItems));
        }

        public bool Contains(MapSpace item)
        {
            return allSpaces.Contains(item);
        }

        public void CopyTo(MapSpace[] array, int arrayIndex)
        {
            allSpaces.CopyTo(array, arrayIndex);
        }

        public IEnumerator<MapSpace> GetEnumerator()
        {
            return allSpaces.GetEnumerator();
        }

        public int IndexOf(MapSpace item)
        {
            return allSpaces.IndexOf(item);
        }

        public void Insert(int index, MapSpace item)
        {
            allSpaces.Insert(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<MapSpace>() { item }, null, index));
        }

        public bool Remove(MapSpace item)
        {
            bool result = allSpaces.Remove(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, null, new List<MapSpace>() { item }));
            return result;
        }

        public void RemoveAt(int index)
        {
            MapSpace removedItem = allSpaces[index];
            allSpaces.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, null, new List<MapSpace>() { removedItem }, index));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allSpaces.GetEnumerator();
        }
        #endregion
    }
}
