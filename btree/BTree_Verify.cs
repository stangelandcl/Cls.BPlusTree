using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace btree
{
    public partial class BTree<TKey, TValue> 
    {
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
                var i = ((Internal<TKey, TValue>)node);
                int j = 0;
                foreach (var x in i.Nodes)
                {
                    if (comparer.Compare(x.Keys[0], i.Keys[j++]) != 0)
                        throw new Exception("Mismatch " + x.Keys[0] + " " + i.Keys[j - 1]);
                    foreach (var item in NodesOf(x))
                        yield return item;
                }
            }
        }

    }
}
