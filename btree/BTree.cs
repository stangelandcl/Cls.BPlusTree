using System;
using System.Collections.Generic;

namespace btree
{
	public class BTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		public BTree (IComparer<TKey> comparer = null)
		{
			this.comparer = this.comparer ?? Comparer<TKey>.Default;
		}
		IComparer<TKey> comparer;
		INode<TKey,TValue> root;

		#region Add
		void AddRoot(TKey key, TValue value){
			this.root = new Leaf<TKey,TValue> ();
			var root = (Leaf<TKey,TValue>)this.root;
			root.Add (key, value, comparer);
		}

		void AddRootLeaf(TKey key, TValue value){
			var r = (Leaf<TKey,TValue>)root;
			if (r.Add (key, value, comparer)) return;
			root = Split (root);
			Add (key, value);
		}
		Internal<TKey,TValue> Split(INode<TKey, TValue> node){
			var left = node;
			var right = node.Split ();
			var r = new Internal<TKey, TValue> ();
			r.Add (left, comparer);
			r.Add (right, comparer);
			return r;
		}

		void AddRootInternal(TKey key, TValue value){
			var r = (Internal<TKey,TValue>)root;
			Internal<TKey,TValue> parent = null;
			AddInternal (ref parent, r, key, value);
			if(parent != null) root = parent;
		}

		void AddInternal(ref Internal<TKey, TValue> parent, Internal<TKey,TValue> node, TKey key, TValue value){
			if (node.IsFull) {
				if (parent == null) {
					parent = new Internal<TKey, TValue> ();
					parent.Add (node, comparer);
				}
				parent.Add (node.Split(), comparer);
				node = parent;
			}

			var next = node.GetNode (key, comparer);
			var asInternal = next as Internal<TKey,TValue>;
			if (asInternal != null)
				AddInternal (ref node, asInternal, key, value);			
			else {
				var r = (Leaf<TKey,TValue>)next;
				if(r.Add(key, value, comparer)) return;
				node.Add (r.Split(), comparer);
				AddInternal (ref parent, node, key, value);
			}
		}

		public void Add(TKey key, TValue value){
			if (root == null) {
				AddRoot (key, value);
				return;
			}

			if(root is Leaf<TKey,TValue>){
				AddRootLeaf (key, value);
				return;
			}

			AddRootInternal (key, value);
		}
		#endregion

		void RemoveRootLeaf(TKey key){
			var leaf = (Leaf<TKey,TValue>)root;
			if (leaf.Remove (key, comparer) && leaf.Count == 0) 
				root = null;
		}

		bool Rebalance(INode<TKey, TValue> parentNode, INode<TKey,TValue> leaf, ref INode<TKey,TValue> newParent, TKey key){
			var parent = (Internal<TKey,TValue>)parentNode;
			if (leaf.Count >= Constants.MinNodeSize)
				return false;

			var left = parent.Left (leaf);
			if (left != null && left.Count > Constants.MinNodeSize) {
				leaf.AddFromRight (left, comparer);			
				parent.Update (leaf);
				return false;
			}

			var right = parent.Right (leaf);
			if (right != null && right.Count > Constants.MinNodeSize) {
				left.AddFromLeft (right, comparer);
				parent.Update (right);
				return false;
			}

			if (left != null) {
				left.AddRange (leaf, comparer);
				parent.Remove (leaf);
				return true;
			}

			if (right != null) {
				right.AddRange (leaf, comparer);
				parent.Remove (leaf);
				return true;
			}

			newParent = leaf;
			return true;
		}

		bool Remove(INode<TKey,TValue> parent, INode<TKey,TValue> child, ref INode<TKey,TValue> newParent, TKey key){
			var leaf = child as Leaf<TKey, TValue>;
			if (leaf == null) {
				var c2 = ((Internal<TKey,TValue>)child).GetNode (key, comparer);
				if (Remove (child, c2, key))
					return Rebalance (parent, child, key);
				return false;
			}

			if (!leaf.Remove (key, comparer)) 
				return false;

			INode<TKey,TValue> n = null;
			bool reb = Rebalance (parent, leaf, ref n, key);
			if(reb){
				if(n != null) 
			}
		}

		void RemoveRootInternal(TKey key){
			var node = (Internal<TKey,TValue>)root;
			var next = node.GetNode (key, comparer);
			INode<TKey,TValue> n = null;
			Remove (node, next, ref n, key);
			if (n != null)	root = n;
		}

		public void Remove(TKey key){
			if(root == null) return;
			if (root is Leaf<TKey,TValue>) {
				RemoveRootLeaf (key);
				return;
			}

			RemoveRootInternal (key);
		}

		IEnumerable<KeyValuePair<TKey,TValue>> NodeItems(INode<TKey, TValue> node){
			if (node is Leaf<TKey,TValue>) 
				foreach (var item in (Leaf<TKey,TValue>)node)
					yield return item;
			else 
				foreach(var n in (Internal<TKey,TValue>)node)
					foreach(var item in NodeItems(n))
						yield return item;
		}
	
		#region IEnumerable implementation
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
		{
			if(root == null) yield break;
			foreach (var item in NodeItems(root))
				yield return item;
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

