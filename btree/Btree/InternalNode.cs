using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace treap
{
	public class InternalNode<TKey, TValue> : INode<TKey,TValue>{
		public InternalNode(){
			Keys = new TKey[Constants.NodeSize -1];
			Nodes = new INode<TKey,TValue>[Keys.Length + 1];			
		}
		public InternalNode(TKey key, INode<TKey,TValue> left, INode<TKey,TValue> right)
			: this()
		{		
			Keys[0] = key;
			Nodes[0] = left;
			Nodes[1] = right;
			Count = 1;
		}
		/// <summary>
		/// In this case the Count of Keys. Count of Nodes is Count + 1
		/// </summary>
		/// <value>The count.</value>
		public int Count {get;set;}
		public int NodeCount {get{return Count +1;}}
		public TKey[] Keys {get; private set;}
		public INode<TKey,TValue>[] Nodes;

		public bool IsFull {get{
				return Count == Keys.Length;
			}
		}

		public void RemoveAt (int nodeIndex)
		{		
			if(nodeIndex < Count)
				Algorithms.RemoveAt(Keys, nodeIndex, Count);	
			else if(nodeIndex == Count && Count != 0) // remove last node means remove last key too
				Algorithms.RemoveAt(Keys, nodeIndex-1, Count);
			Algorithms.RemoveAt(Nodes, nodeIndex, Count+1);
			Count--;
		}

		
		public void RemoveRange(int index, int count){
			Algorithms.RemoveRange(Keys, index, count, Count);
			Algorithms.RemoveRange(Nodes, index, count, Count+1);
			Count -= count;
		}

		public void AddRight(INode<TKey,TValue> node, int count){
			var leaf = (InternalNode<TKey,TValue>)node;
			for(int i=Count, j=0;j <count;i++,j++)
				Keys[i] = leaf.Keys[j];
			for(int i=Count, j=0;j <count+1;i++,j++)
				Nodes[i] = leaf.Nodes[j];
			node.RemoveRange(0, count);
			Count += count;
		}

		public void AddLeft(INode<TKey,TValue> node, int count){
			var leaf = (InternalNode<TKey,TValue>)node;
			Algorithms.Copy(Keys,0, Keys, count, Count);
			Algorithms.Copy(Nodes,0, Nodes, count, Count + 1);

			var start = leaf.Count - count;
			for(int i=0,j=start;i< count ;i++, j++){
				Keys[i] = leaf.Keys[j];
				Nodes[i] = leaf.Nodes[j];
			}		
			node.RemoveRange(start, count);
			Count+=count;
		}

		public void Add(TKey key, INode<TKey,TValue> right, IComparer<TKey> comparer){
			var index = IndexOf(key, comparer);
			Algorithms.Insert(Keys, key, index, Count);
			Algorithms.Insert(Nodes, right, index+1, NodeCount);
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
			Algorithms.Copy(Keys, half+1, right.Keys,0, half);
			Algorithms.Copy(Nodes, half+1, right.Nodes,0, half + 1);
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

