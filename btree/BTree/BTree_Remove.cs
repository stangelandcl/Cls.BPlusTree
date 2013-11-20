using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace btree
{
    public partial class BTreeDictionary<TKey, TValue> 
    {
        public void Remove(TKey key)
        {
            if (root == null)
                return;

            if (root is Leaf<TKey, TValue>)
            {
                RemoveRootLeaf(key);
                return;
            }

            RemoveRootInternal(key);
        }

        void RemoveRootLeaf(TKey key)
        {
            var leaf = (Leaf<TKey, TValue>)root;
            if (leaf.Remove(key, comparer) && leaf.Count == 0)
                root = null;
        }

        void RemoveRootInternal(TKey key)
        {
            var node = (Internal<TKey, TValue>)root;
            var next = node.GetNode(key, comparer);
            var removed = Remove(node, next.Node, next.Index, key);
            if (removed.Rebalanced)
            {
                if (removed.NewParent != null)
                    root = removed.NewParent;
            }
        }

        RebalanceResult Remove(INode<TKey, TValue> parent, INode<TKey, TValue> child, int childIndex, TKey key)
        {
            var leaf = child as Leaf<TKey, TValue>;
            if (leaf != null)
            {
                if (!leaf.Remove(key, comparer))
                    return RebalanceResult.Empty;
                ((Internal<TKey, TValue>)parent).Update(childIndex, child);
                return Rebalance(parent, leaf, childIndex);
            }

            var grandChild = ((Internal<TKey, TValue>)child).GetNode(key, comparer);
            var removed = Remove(child, grandChild.Node, grandChild.Index, key);
            ((Internal<TKey, TValue>)parent).Update(childIndex, child);
            if (removed.Rebalanced)
            {
                if (removed.NewParent != null)
                {
                    ((Internal<TKey, TValue>)parent).Replace(childIndex, grandChild.Node);
                    child = grandChild.Node;
                }
                return Rebalance(parent, child, childIndex);
            }
            return RebalanceResult.Empty;
        }       
       
        RebalanceResult Rebalance(INode<TKey, TValue> parentNode, INode<TKey, TValue> leaf, int leafIndex)
        {
            var parent = (Internal<TKey, TValue>)parentNode;

            if (leaf.Count >= Constants.MinNodeSize)
                return RebalanceResult.Empty;

            var left = parent.Left(leafIndex);
            if (left != null && left.Count > Constants.MinNodeSize)
            {
                leaf.AddFromRight(left, comparer);
                parent.Update(leafIndex, leaf);
                return RebalanceResult.Empty;
            }

            var right = parent.Right(leafIndex);
            if (right != null && right.Count > Constants.MinNodeSize)
            {
                leaf.AddFromLeft(right, comparer);
                parent.Update(leafIndex +1, right);
                return RebalanceResult.Empty;
            }

            if (left != null)
            {
                left.AddRange(leaf, comparer);
                parent.Update(leafIndex-1, left);
                parent.Remove(leafIndex);
                return new RebalanceResult { Rebalanced = true };
            }

            if (right != null)
            {
                right.AddRange(leaf, comparer);
                parent.Update(leafIndex+1, right);
                parent.Remove(leafIndex);
                return new RebalanceResult { Rebalanced = true };
            }
            return new RebalanceResult
            {
                NewParent = leaf,
                Rebalanced = true,
            };
        }

        struct RebalanceResult
        {
            public bool Rebalanced;
            public INode<TKey, TValue> NewParent;
            public static readonly RebalanceResult Empty = new RebalanceResult();
        }
    }
}
