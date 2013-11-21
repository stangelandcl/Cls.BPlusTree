using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTree
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
            var removed = Remove(node, next, key);
            if (removed.Rebalanced)
            {
                if (removed.NewParent != null)
                    root = removed.NewParent;
            }
        }

        RebalanceResult Remove(INode<TKey, TValue> parent, NodeIndex<TKey,TValue> child,  TKey key)
        {
            var leaf = child.Node as Leaf<TKey, TValue>;
            if (leaf != null)
            {
                if (!leaf.Remove(key, comparer))
                    return RebalanceResult.Empty;
                ((Internal<TKey, TValue>)parent).Update(child.Index, child.Node);
                return Rebalance(parent, child);
            }

            var grandChild = ((Internal<TKey, TValue>)child.Node).GetNode(key, comparer);
            var removed = Remove(child.Node, grandChild, key);
            ((Internal<TKey, TValue>)parent).Update(child.Index, child.Node);
            if (removed.Rebalanced)
            {
                if (removed.NewParent != null)
                {
                    ((Internal<TKey, TValue>)parent).Replace(child.Index, grandChild.Node);
                    child.Node = grandChild.Node;
                }
                return Rebalance(parent, child);
            }
            return RebalanceResult.Empty;
        }       
       
        RebalanceResult Rebalance(INode<TKey, TValue> parentNode, NodeIndex<TKey,TValue> child)
        {
            var parent = (Internal<TKey, TValue>)parentNode;

            if (child.Node.Count >= Constants.MinNodeSize)
                return RebalanceResult.Empty;

            var left = parent.Left(child.Index);
            if (left != null && left.Count > Constants.MinNodeSize)
            {
                child.Node.AddFromRight(left, comparer);
                parent.Update(child.Index, child.Node);
                return RebalanceResult.Empty;
            }

            var right = parent.Right(child.Index);
            if (right != null && right.Count > Constants.MinNodeSize)
            {
                child.Node.AddFromLeft(right, comparer);
                parent.Update(child.Index +1, right);
                return RebalanceResult.Empty;
            }

            if (left != null)
            {
                left.AddRange(child.Node, comparer);
                parent.Update(child.Index-1, left);
                parent.Remove(child.Index);
                return new RebalanceResult { Rebalanced = true };
            }

            if (right != null)
            {
                right.AddRange(child.Node, comparer);
                parent.Update(child.Index+1, right);
                parent.Remove(child.Index);
                return new RebalanceResult { Rebalanced = true };
            }
            return new RebalanceResult
            {
                NewParent = child.Node,
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
