using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace btree
{
    public partial class BTreeDictionary<TKey, TValue> 
    {
        public void Add(TKey key, TValue value)
        {
            if (root == null)
            {
                AddRoot(key, value);
                return;
            }

            if (root is Leaf<TKey, TValue>)
            {
                AddRootLeaf(key, value);
                return;
            }

            AddRootInternal(key, value);
        }     

        void AddRoot(TKey key, TValue value)
        {
            this.root = new Leaf<TKey, TValue>();
            var root = (Leaf<TKey, TValue>)this.root;
            root.Add(key, value, comparer);
        }

        void AddRootLeaf(TKey key, TValue value)
        {
            var r = (Leaf<TKey, TValue>)root;
            if (r.Add(key, value, comparer)) return;
            root = Split(root);
            Add(key, value);
        }

        Internal<TKey, TValue> Split(INode<TKey, TValue> node)
        {
            var left = node;
            var right = node.Split();
            var r = new Internal<TKey, TValue>();
            r.Add(left, comparer);
            r.Add(right, comparer);
            return r;
        }

        void AddRootInternal(TKey key, TValue value)
        {
            var r = (Internal<TKey, TValue>)root;
            Internal<TKey, TValue> parent = null;
            AddInternal(ref parent, r, key, value);
            if (parent != null) root = parent;
        }

        void AddInternal(ref Internal<TKey, TValue> parent, Internal<TKey, TValue> node, TKey key, TValue value)
        {
            if (node.IsFull)
            {
                if (parent == null)
                {
                    parent = new Internal<TKey, TValue>();
                    parent.Add(node, comparer);
                }
                parent.Add(node.Split(), comparer);
                node = parent;
            }

            var next = node.GetNode(key, comparer);
            node.UpdateNode(next.Index, key, comparer);
            var asInternal = next.Node as Internal<TKey, TValue>;
            if (asInternal != null)
                AddInternal(ref node, asInternal, key, value);
            else
            {
                var r = (Leaf<TKey, TValue>)next.Node;
                if (r.Add(key, value, comparer)) return;
                node.Add(r.Split(), comparer);
                AddInternal(ref parent, node, key, value);
            }
        }           
    }
}
