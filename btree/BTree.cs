using System;
using System.Collections.Generic;
using System.Linq;

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

        struct RebalanceResult
        {
            public bool Rebalanced;
            public INode<TKey, TValue> NewParent;
            public static readonly RebalanceResult Empty = new RebalanceResult();
        }

		RebalanceResult Rebalance(INode<TKey, TValue> parentNode, INode<TKey,TValue> leaf, TKey key){
			var parent = (Internal<TKey,TValue>)parentNode;
            if (leaf.Count >= Constants.MinNodeSize)
                return RebalanceResult.Empty;

			var left = parent.Left (leaf);
			if (left != null && left.Count > Constants.MinNodeSize) {
				leaf.AddFromRight (left, comparer);			
				parent.Update (leaf);
				return RebalanceResult.Empty;
			}

			var right = parent.Right (leaf);
			if (right != null && right.Count > Constants.MinNodeSize) {
				leaf.AddFromLeft (right, comparer);
				parent.Update (right);
				return RebalanceResult.Empty;
			}

			if (left != null) {
				left.AddRange (leaf, comparer);
				parent.Remove (leaf);
                return new RebalanceResult { Rebalanced = true };
			}

			if (right != null) {
				right.AddRange (leaf, comparer);
				parent.Remove (leaf);
                return new RebalanceResult { Rebalanced = true } ;
			}
            return new RebalanceResult
            {
                NewParent = leaf,
                Rebalanced = true,
            };			
		}

		RebalanceResult Remove(INode<TKey,TValue> parent, INode<TKey,TValue> child, TKey key){
			var leaf = child as Leaf<TKey, TValue>;
            if (leaf != null)
            {
                if (!leaf.Remove(key, comparer))
                    return RebalanceResult.Empty;
            
                return Rebalance(parent, leaf, key);
            }
			
			var grandChild = ((Internal<TKey,TValue>)child).GetNode (key, comparer);
			var removed = Remove (child, grandChild, key);
            if (removed.Rebalanced)
            {
                if (removed.NewParent != null)
                {
                    ((Internal<TKey,TValue>)parent).Replace(child, grandChild);
                }
                return Rebalance(parent, child, key);
            }
            return RebalanceResult.Empty;										
		}

		void RemoveRootInternal(TKey key){
			var node = (Internal<TKey,TValue>)root;
			var next = node.GetNode (key, comparer);			
            var removed = Remove(node, next, key);
            if (removed.Rebalanced)
            {
                if (removed.NewParent != null)                
                    root = removed.NewParent;                
            }			
		}

		public void Remove(TKey key){
			if(root == null) 
                return;

			if (root is Leaf<TKey,TValue>) {
				RemoveRootLeaf (key);
				return;
			}

			RemoveRootInternal (key);                      
		}

        public bool Verify()
        {
            if (root == null) return true;
            foreach (var node in this.Nodes)
                if (node != root && node.Keys.Count < Constants.MinNodeSize)
                    return false;
            return true;
        }

        IEnumerable<INode<TKey, TValue>> Nodes
        {
            get
            {
                if (root == null) 
                    return Enumerable.Empty<INode<TKey, TValue>>();
                return NodesOf(root);
            }
        }

        IEnumerable<INode<TKey, TValue>> NodesOf(INode<TKey, TValue> node)
        {
            var n = node as Leaf<TKey, TValue>;
            if (n != null)
                yield return n;
            else
            {
                yield return node;
                foreach (var x in ((Internal<TKey, TValue>)node).Nodes)
                    foreach (var item in NodesOf(x))
                        yield return item;
            }
        }

		IEnumerable<KeyValuePair<TKey,TValue>> NodeItems(INode<TKey, TValue> node){
            var n = node as Leaf<TKey, TValue>;
			if (n != null) 
				foreach (var item in n)
					yield return item;
			else 
				foreach(var x in (Internal<TKey,TValue>)node)
					foreach(var item in NodeItems(x))
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

