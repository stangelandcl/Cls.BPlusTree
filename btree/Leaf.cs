using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace btree
{
    [DebuggerDisplay("Keys={Keys.Count}")]
	public class Leaf<TKey, TValue> : INode<TKey, TValue>, IEnumerable<KeyValuePair<TKey,TValue>>
	{
		public Leaf(){
			Keys = new List<TKey> ();
		}
		public void AddRange(IEnumerable<KeyValuePair<TKey,TValue>> kvp, IComparer<TKey> comparer){
			foreach (var k in kvp)
				Add (k.Key, k.Value, comparer);
		}
		public void AddRange(INode<TKey,TValue> node, IComparer<TKey> comparer)
        {
			AddRange (((Leaf<TKey,TValue>)node).AsEnumerable(), comparer);
		}
		public void AddFromLeft(INode<TKey,TValue> node, IComparer<TKey> comparer)
        {
			AddRange (((Leaf<TKey,TValue>)node).TakeLeft (), comparer);
		}
		public void AddFromRight(INode<TKey,TValue> node, IComparer<TKey> comparer)
        {
			AddRange (((Leaf<TKey,TValue>)node).TakeRight (), comparer);
		}

		public KeyValuePair<TKey,TValue>[] TakeLeft(){
			var count = Math.Max ((Constants.NodeSize - Count) / 2, 1);
			var items = new KeyValuePair<TKey,TValue>[count];
			for (int i=0; i<items.Length; i++)
				items [i] = new KeyValuePair<TKey, TValue> (Keys [i], values [i]);
			Keys.RemoveRange (0, count);
			values.RemoveRange (0, count);
			return items;
		}
		public KeyValuePair<TKey,TValue>[] TakeRight(){
			var count = Math.Max ((Count - Constants.MinNodeSize) / 2, 1);
			var items = new KeyValuePair<TKey,TValue>[count];
			for(int i=0;i<items.Length;i++){
				var x = Keys.Count - count + i;
				items [i] = new KeyValuePair<TKey, TValue> (Keys [x], values [x]);
			}
			Keys.RemoveRange (Keys.Count - count, count);
			values.RemoveRange (values.Count - count, count);
			return items;
		}

		public bool Add(TKey key, TValue value, IComparer<TKey> comparer){
			var index = Keys.BinarySearch (key, comparer);
			if (index >= 0) {
				Keys [index] = key;
				values [index] = value;
				return true;
			}

			if(Keys.Count == Constants.NodeSize) return false;
			index = ~index;
								
			Keys.Insert (index, key);
			values.Insert (index, value);
			return true;
		}

		public INode<TKey,TValue> Split(){
			var right = new Leaf<TKey,TValue> ();
			var count = Constants.NodeSize / 2;
			right.Keys.AddRange (Keys.GetRange (count, count));
			Keys.RemoveRange (count, count);
			right.values.AddRange (values.GetRange (count, count));
			values.RemoveRange (count, count);
			return right;
		}
		public int Count {get{return Keys.Count;}}
		public List<TKey> Keys {get; private set;}

		public bool Remove(TKey key, IComparer<TKey> comparer){
			var index = Keys.BinarySearch (key, comparer);
			if(index < 0) return false;
			Keys.RemoveAt (index);
			values.RemoveAt (index);
			return true;
		}

		#region IEnumerable implementation


		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
		{
			for (int i=0; i<Keys.Count; i++)
				yield return new KeyValuePair<TKey,TValue> (Keys [i], values [i]);
		}


		#endregion


		#region IEnumerable implementation


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}


		#endregion

		List<TValue> values = new List<TValue>();
	}
}

