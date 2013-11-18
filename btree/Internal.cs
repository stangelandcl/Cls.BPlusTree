using System;
using System.Collections.Generic;
using System.Linq;

namespace btree
{
	public class Internal<TKey,TValue> : INode<TKey, TValue>, IEnumerable<INode<TKey,TValue>>
	{
		public Internal(){
			Keys = new List<TKey> ();
		}

		public void AddRange(IEnumerable<INode<TKey,TValue>> nodes, IComparer<TKey> comparer){
			foreach (var node in nodes)
				Add (node, comparer);
		}

		public void AddRange(INode<TKey,TValue> node, IComparer<TKey> comparer){
			AddRange (((Internal<TKey,TValue>)node).AsEnumerable(), comparer);
		}

		public void AddFromLeft(INode<TKey,TValue> node, IComparer<TKey> comparer){
			AddRange (((Internal<TKey,TValue>)node).TakeLeft (), comparer);
		}
		public void AddFromRight(INode<TKey,TValue> node, IComparer<TKey> comparer){
			AddRange (((Internal<TKey,TValue>)node).TakeRight (), comparer);
		}

		public bool Add(INode<TKey,TValue> node, IComparer<TKey> comparer){
			var index = Keys.BinarySearch (node.Keys [0], comparer);
			if (index >= 0) {
				Keys [index] = node.Keys [0];
				nodes [index] = node;
				return true;
			}
			
			if(Keys.Count == Constants.NodeSize) return false;
			index = ~index;

			Keys.Insert (index, node.Keys [0]);
			nodes.Insert (index, node);
			return true;
		}

		public bool IsFull{ get { return Keys.Count == Constants.NodeSize; } }

		public INode<TKey,TValue> GetNode(TKey key, IComparer<TKey> comparer){
			var index = Keys.BinarySearch (key, comparer);
			if (index < 0) {
				index = ~index;
				if(index > 0) index --;
			}
			if (comparer.Compare (key, Keys [index]) < 0)
				Keys [index] = key;
			return nodes [index];
		}

		public INode<TKey,TValue> Split(){
			var right = new Internal<TKey,TValue> ();
			var count = Constants.NodeSize / 2;
			right.Keys.AddRange (Keys.GetRange (count, count));
			Keys.RemoveRange (count, count);
			right.nodes.AddRange (nodes.GetRange (count, count));
			nodes.RemoveRange (count, count);
			return right;
		}

		public int Count { get { return Keys.Count; } }
		public List<TKey> Keys {get; private set;}
		List<INode<TKey,TValue>> nodes = new List<INode<TKey, TValue>>();

		public INode<TKey,TValue> Left(INode<TKey,TValue> node){
			var i = nodes.IndexOf (node) - 1;
			if(i >= 0) return nodes[i];
			return null;
		}

		public INode<TKey,TValue> Right(INode<TKey,TValue> node){
			var i = nodes.IndexOf (node) + 1;
			if(i < nodes.Count) return nodes[i];
			return null;
		}

		public void Update(INode<TKey,TValue> node){
			var i = nodes.IndexOf (node);
			Keys [i] = node.Keys [0];
		}

		public void Remove(INode<TKey,TValue> node){
			var index = nodes.IndexOf (node);
			Keys.RemoveAt (index);
			nodes.RemoveAt (index);
		}

		public INode<TKey,TValue>[] TakeLeft(){
			var count = Math.Max ((Constants.NodeSize - Count) / 2, 1);
			var items = new INode<TKey,TValue>[count];
			for (int i=0; i<items.Length; i++)
				items [i] = nodes [i];
			Keys.RemoveRange (0, count);
			nodes.RemoveRange (0, count);
			return items;
		}
		public INode<TKey,TValue>[] TakeRight(){
			var count = Math.Max ((Constants.NodeSize - Count) / 2, 1);
			var items = new INode<TKey,TValue>[count];
			for(int i=0;i<items.Length;i++){
				var x = Keys.Count - count + i;
				items [i] = nodes [x];
			}
			Keys.RemoveRange (Keys.Count - count, count);
			nodes.RemoveRange (nodes.Count - count, count);
			return items;
		}


		#region IEnumerable implementation

		public IEnumerator<INode<TKey, TValue>> GetEnumerator ()
		{
			return nodes.GetEnumerator ();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		#endregion
	}
}

