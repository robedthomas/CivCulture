using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Utilities
{
    /// <summary>
    /// An observable collection with dictionary behavior
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public class ObservableDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Fields
        private bool isReadOnly = false;
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        protected Dictionary<TKey, TValue> SourceDict { get; set; }

        public int Count
        {
            get => SourceDict.Count;
        }

        public bool IsReadOnly
        {
            get => isReadOnly;
            protected set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReadOnly)));
                }
            }
        }
        #endregion

        #region Constructors
        public ObservableDictionary()
        {
            SourceDict = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(bool isReadOnly) : this()
        {
            IsReadOnly = isReadOnly;
        }

        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this()
        {
            if (collection != null)
            {
                SourceDict = new Dictionary<TKey, TValue>(collection);
            }
        }
        #endregion

        #region Methods
        public TValue this[TKey key]
        {
            get => SourceDict[key];
            set
            {
                TValue oldValue = default(TValue);
                bool alreadyPresent = SourceDict.ContainsKey(key);
                if (alreadyPresent)
                {
                    oldValue = SourceDict[key];
                }
                SourceDict[key] = value;
                if (alreadyPresent)
                {
                    int index = SourceDict.ToList().IndexOf(new KeyValuePair<TKey, TValue>(key, value));
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    new KeyValuePair<TKey, TValue>(key, value),
                    new KeyValuePair<TKey, TValue>(key, oldValue),
                    index
                    ));
                }
                else
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    new List<KeyValuePair<TKey, TValue>>() { new KeyValuePair<TKey, TValue>(key, SourceDict[key]) }
                    ));
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            SourceDict.Add(key, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                new Dictionary<TKey, TValue>() { { key, value } }
                ));
        }

        public void Clear()
        {
            Dictionary<TKey, TValue> tempCopy = new Dictionary<TKey, TValue>(SourceDict);
            SourceDict.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset,
                null,
                tempCopy
                ));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item.Key);
        }

        public bool Contains(TKey key)
        {
            return SourceDict.ContainsKey(key);
        }

        public bool Contains(TValue value)
        {
            return SourceDict.ContainsValue(value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(paramName : nameof(array));
            }
            else if (arrayIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException("Given array index is greater than length of buffer array");
            }
            else if (array.Length - arrayIndex < SourceDict.Count)
            {
                throw new ArgumentException("Given array index does not leave enough space in buffer array to copy all of this collection's contents");
            }
            else
            {
                List<KeyValuePair<TKey, TValue>> tempList = this.ToList();
                for (int i = arrayIndex; i < array.Length; i++)
                {
                    array[i] = tempList[i - arrayIndex];
                }
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            TValue removedValue;
            if (SourceDict.TryGetValue(key, out removedValue))
            {
                bool res = SourceDict.Remove(key);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    new Dictionary<TKey, TValue>() { { key, removedValue } }
                    ));
                return res;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return SourceDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SourceDict.GetEnumerator();
        }
        #endregion
    }
}
