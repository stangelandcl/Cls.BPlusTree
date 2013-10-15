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
		INode root;	
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

		void Remove(TKey key){
			if(root == null) return;		

			INode parent = null;
			Remove(ref root, key, ref parent);
			if(parent != null)
				root = parent;
		}

		void Remove(ref INode node, TKey key, ref INode parent){
			var leaf = node as LeafNode<TKey, TValue>;
			if(leaf != null){
				if(leaf.Remove(key, comparer))
				{
					if(leaf.Count == 0)
				}
			}else{

			}
		}

		void Add(ref INode node, TKey key, TValue value, ref INode parent, bool overwrite){
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
						INode r =right;
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
				INode r = leafSplit.Right;
				Add(ref r, key, value, ref parent, overwrite);
			}
		}

		void Insert(TKey key, TValue value, bool overwrite){
			INode parent = null;
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
			return ContainsKey(root, key);
		}

		bool ContainsKey(INode node, TKey key){
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

		bool TryGetValue(INode node, TKey key, out TValue value){
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

		IEnumerable<KeyValuePair<TKey,TValue>> Next(INode node){
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

