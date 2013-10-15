using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace treap
{
	public class InternalNode<TKey, TValue> : INode{
		public InternalNode(){
			Keys = new TKey[Constants.NodeSize -1];
			Nodes = new INode[Keys.Length + 1];			
		}
		public InternalNode(TKey key, INode left, INode right)
			: this()
		{		
			Keys[0] = key;
			Nodes[0] = left;
			Nodes[1] = right;
			Count = 1;
		}

		public int Count;
		public TKey[] Keys;
		public INode[] Nodes;

		public bool IsFull {get{
				return Count == Keys.Length;
			}
		}

		public void Add(TKey key, INode right, IComparer<TKey> comparer){
			var index = IndexOf(key, comparer);
			Algorithms.Insert(Keys, key, index, Count);
			Algorithms.Insert(Nodes, right, index+1, Count+1);
			Count++;
		}

		/// <summary>
		/// Returns index of key or the next index greater than key if key not found.
		/// </summary>
		/// <returns>The of.</returns>
		/// <param name="key">Key.</param>
		public int IndexOf(TKey key, IComparer<TKey> comparer){
			var index = Array.BinarySearch(Keys, 0, Count, key, comparer);
			if(index <0) index = ~index;
			return index;
		}
		[Conditional("DEBUG")]
		void ClearRightHalf(){
			for(int i=Count;i<Keys.Length;i++)
				Keys[i] = default(TKey);
			for(int i=Count+1;i<Nodes.Length;i++)
				Nodes[i] = null;
		}

		public InternalSplit<TKey, TValue> Split(){		
			Count /= 2;
			var half = Count;
			var right = new InternalNode<TKey,TValue>();
			Array.Copy(Keys, half+1, right.Keys,0, half);
			Array.Copy(Nodes, half+1, right.Nodes,0, half + 1);
			var middle = Keys[half];
			ClearRightHalf();
			right.Count = this.Count;
			return new InternalSplit<TKey,TValue>{
				Right = right,
				Middle = middle,
			};
		}			
	}

	public struct InternalSplit<TKey,TValue>{
		public InternalNode<TKey,TValue> Right;
		public TKey Middle;
	}
}

