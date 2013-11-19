using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace btree
{
    public partial class BTree<TKey, TValue>
    {

        public bool ContainsKey(TKey key)
        {
            TValue value;
            return TryGetValue(key, out value);
        }

        public ICollection<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            bool contains = ContainsKey(key);
            Remove(key);
            return contains;
        }

        public ICollection<TValue> Values
        {
            get { throw new NotImplementedException(); }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            root = null;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var item in this)
                array[arrayIndex++] = item;
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)this).Remove(item.Key);
        }

    }
}
