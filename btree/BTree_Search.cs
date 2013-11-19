using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace btree
{
    public partial class BTree<TKey, TValue> 
    {
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (root == null)
            {
                value = default(TValue);
                return false;
            }          

            return TryGetValue(key, out value, root);            
        }        

        bool TryGetValue(TKey key, out TValue value, INode<TKey, TValue> node)
        {
            var leaf = node as Leaf<TKey, TValue>;
            if (leaf != null)
                return leaf.TryGetValue(key, out value, comparer);
            var i = (Internal<TKey,TValue>)node;
            return TryGetValue(key, out value, i.GetNode(key, false, comparer).Node);            
        }

        internal KeyPosition<TKey, TValue> MoveTo(TKey key)
        {
            var nodes = new KeyPosition<TKey, TValue>();
            if (root == null)
                return nodes;

            MoveTo(key, nodes, root);
            return nodes;
        }

        void MoveTo(TKey key, KeyPosition<TKey, TValue> nodes, INode<TKey, TValue> node)
        {           
            var i = node as Internal<TKey, TValue>;
            if (i == null)
            {
                var val = i.GetNode(key, false, comparer);
                nodes.Push(KeyIndex.New(val.Index, node));
                MoveTo(key, nodes, val.Node);
                return;
            }

            var leaf = (Leaf<TKey, TValue>)node;
            var index = leaf.Keys.BinarySearch(key, comparer);
            if (index < 0) index = ~index;
            nodes.Push(KeyIndex.New(index, node));            
        }
    }
}
