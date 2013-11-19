using System;
using System.Collections.Generic;
using System.Linq;

namespace btree
{
	public partial class BTree<TKey, TValue> : IDictionary<TKey,TValue>
	{
		public BTree (IComparer<TKey> comparer = null)
		{
			this.comparer = this.comparer ?? Comparer<TKey>.Default;
		}
		IComparer<TKey> comparer;
		INode<TKey,TValue> root;

        IEnumerable<KeyValuePair<TKey, TValue>> NodeItems(INode<TKey, TValue> node)
        {
            var n = node as Leaf<TKey, TValue>;
            if (n != null)
                foreach (var item in n)
                    yield return item;
            else
                foreach (var x in (Internal<TKey, TValue>)node)
                    foreach (var item in NodeItems(x))
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

