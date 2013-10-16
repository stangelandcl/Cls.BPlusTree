using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace treap
{
	public class BTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		public BTree () : this(Comparer<TKey>.Default){}
		public BTree (IComparer<TKey> comparer)
		{
			this.comparer = comparer;
			this.likeComparer = new LikeComparer<TKey>(comparer);
		}

		IComparer<TKey> comparer;
		INode<TKey,TValue> root;	
		LikeComparer<TKey> likeComparer;

		#region ICollection implementation

		public TValue this[TKey key]{
			get{
				return default(TValue);
			}
			set{
				Insert(key, value, true);
			}
		}

		public void Remove(TKey key){
			if(root == null) return;
			INode<TKey,TValue> parent = null;
			Remove(ref root, key, parent, 0);
		}

		void Rebalance(ref INode<TKey, TValue> node, INode<TKey, TValue> parent, int index){
			if(node.Count >= Constants.MinimumSize)
				return;

			if(parent == null){ // node is root
				if(node.Count == 0){
					var inode = node as InternalNode<TKey,TValue>;
					if(inode != null)
						node = inode.Nodes[0]; // move node up a level
					else
						node = null; // leaf node is empty means root is empty.
				}
				return;
			}

			var iparent = (InternalNode<TKey,TValue>)parent;
			if(index > 0){
				var leftIndex = index -1;
				var left = iparent.Nodes[leftIndex];
				if(left.Count > Constants.MinimumSize){
					node.AddLeft(left, Math.Max(1, (left.Count - Constants.MinimumSize) / 2));
					//if(leftIndex < iparent.Count) // always true
					iparent.Keys[leftIndex] = left.Keys[left.Count-1];
					return;
				}
			}else if(index < iparent.Count-1){
				var rightIndex = index+1;
				var right = iparent.Nodes[rightIndex];
				if(right.Count > Constants.MinimumSize){
					node.AddRight(right, Math.Max(1, (right.Count - Constants.MinimumSize) / 2));
					iparent.Keys[index] = node.Keys[node.Count-1];
					return;
				}
			}

			// could not steal from sibling, so merge with one instead
			if(index > 0){
				var leftIndex = index -1;
				var left = iparent.Nodes[leftIndex];
				left.AddRight(node, node.Count);
				iparent.Keys[leftIndex] = left.Keys[left.Count-1];
				iparent.RemoveAt(index);
				return;
			}else if(index < iparent.Count -1){
				var rightIndex = index + 1;
				var right = iparent.Nodes[rightIndex];
				right.AddLeft(node, node.Count);
				iparent.RemoveAt(index);
				node = null;
				return;
			}
		}

		void Remove(ref INode<TKey,TValue> node, TKey key, INode<TKey,TValue> parent, int index){
			var leaf = node as LeafNode<TKey, TValue>;
			if(leaf == null){
				var inode = (InternalNode<TKey,TValue>)node;
				var keyIndex = inode.IndexOf(key, comparer);
				var next = inode.Nodes[keyIndex];
				Remove(ref next, key, node, keyIndex);
				//if(node.Count < Constants.MinimumSize && parent != null || node.Count == 0)
				Rebalance(ref node, parent, index);
				return;
			}

			if(!leaf.Remove(key, comparer))
				return;

			Rebalance(ref node, parent, index);
		}

		void Add(ref INode<TKey,TValue> node, TKey key, TValue value, ref INode<TKey,TValue> parent, bool overwrite){
			if(node == null){
				node = new LeafNode<TKey,TValue>(key, value);
				Count++;
				return;
			}

			int index;
			var inode = node as InternalNode<TKey,TValue>;
			if(inode != null){
				if(inode.IsFull){
					index = inode.IndexOf(key, comparer);
					var split = inode.Split();
					var left = node;
					var right = split.Right;
					var middleKey = split.Middle;
					if(parent == null){
						parent = new InternalNode<TKey, TValue>(middleKey, left, right);
					}else{
						var iparent = (InternalNode<TKey,TValue>)parent;
						iparent.Add(middleKey, right, comparer);
					}
					if(index < inode.Count)
						Add(ref left, key, value, ref parent, overwrite);
					else{
						INode<TKey,TValue> r =right;
						Add(ref r, key, value, ref parent, overwrite);
					}
					return;
				}
				index = inode.IndexOf(key, comparer);
				var next = inode.Nodes[index];
				Add(ref next, key, value, ref node, overwrite);
				return;
			}

			var leaf = (LeafNode<TKey, TValue>)node;
			if(leaf.Add(key, value, comparer, overwrite))
				return;

			index = leaf.IndexOf(key, comparer);
			var leafSplit = leaf.Split();
			if(parent == null){
				parent = new InternalNode<TKey, TValue>(leafSplit.Middle, node, leafSplit.Right);
			}else{
				var iparent = (InternalNode<TKey,TValue>)parent;
				iparent.Add(leafSplit.Middle, leafSplit.Right, comparer);
			}
			if(index < leaf.Count)
				Add(ref node, key, value, ref parent, overwrite);
			else{
				INode<TKey,TValue> r = leafSplit.Right;
				Add(ref r, key, value, ref parent, overwrite);
			}
		}

		void Insert(TKey key, TValue value, bool overwrite){
			INode<TKey,TValue> parent = null;
			Add(ref root, key, value, ref parent, overwrite);
			if(parent != null)
				root = parent;
		}

		public void Add (TKey key, TValue value)
		{
			Insert(key, value, false);
		}

		public void Clear ()
		{
			this.root = null;
			Count = 0;
		}

		public bool ContainsKey (TKey key)
		{
			if(root == null) return false;
			return ContainsKey(root, key);
		}

		bool ContainsKey(INode<TKey,TValue> node, TKey key){
			var n = node as InternalNode<TKey, TValue>;
			if(n != null){
				var index = n.IndexOf(key, comparer);
				return ContainsKey(n.Nodes[index], key);
			}

			var leaf = (LeafNode<TKey, TValue>)node;
			return leaf.ContainsKey(key, comparer);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return TryGetValue(root, key, out value);
		}

		bool TryGetValue(INode<TKey,TValue> node, TKey key, out TValue value){
			var n = node as InternalNode<TKey, TValue>;
			if(n != null){
				var index = n.IndexOf(key, comparer);
				return TryGetValue(n.Nodes[index], key, out value);
			}

			var leaf = (LeafNode<TKey, TValue>)node;
			var i = leaf.IndexOf(key, comparer);
			if(i >= 0){
				value = leaf.Values[i];
				return true;
			}
			value = default(TValue);
			return false;
		}

		public int Count { get; private set;}
		public bool IsReadOnly { get { return false; } }

		#endregion

		#region IEnumerable implementation
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
		{
			return Next(root).GetEnumerator();
		}

		IEnumerable<KeyValuePair<TKey,TValue>> Next(INode<TKey,TValue> node){
			if(node == null) yield break;
			var x = node as InternalNode<TKey,TValue>;
			if(x != null){
				for(int i=0;i<x.Count+1;i++){
					foreach(var n in Next(x.Nodes[i]))
						yield return n;
				}
			}else{
				var leaf = (LeafNode<TKey,TValue>)node;
				for(int i=0;i<leaf.Count;i++)
					yield return new KeyValuePair<TKey, TValue>(
						leaf.Keys[i], leaf.Values[i]);
			}
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		#endregion

	}
}

