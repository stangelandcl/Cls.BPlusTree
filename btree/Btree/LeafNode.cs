using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace treap
{
	public class LeafNode<TKey, TValue> : INode<TKey,TValue>{
		public LeafNode(){
			Keys = new TKey[Constants.NodeSize];
			Values = new TValue[Keys.Length];
		}

		public LeafNode(TKey key, TValue value)
			: this()
		{
			Keys[0] = key;
			Values[0] = value;
			Count =1;			
		}

		public int Count {get;set;}
		public TKey[] Keys {get; private set;}
		public TValue[] Values;
		public TKey FarRightKey() { return Keys[Count-1]; }

		public void RemoveAt(int index){
			Algorithms.RemoveAt(Keys, index, Count);
			Algorithms.RemoveAt(Values, index, Count);
			Count--;
		}

		public void AddRight(INode<TKey,TValue> node, int count){
			var leaf = (LeafNode<TKey,TValue>)node;
			for(int i=Count, j=0;i<Count + count && j <count;i++,j++){
				Keys[i] = leaf.Keys[j];
				Values[i] = leaf.Values[j];
			}
			node.RemoveRange(0, count);
			Count += count;
		}

		public void AddLeft(INode<TKey,TValue> node, int count){
			var leaf = (LeafNode<TKey,TValue>)node;
			Algorithms.Copy(Keys,0, Keys, count, Count);
			Algorithms.Copy(Values,0, Values, count, Count);

			var start = leaf.Count - count;
			for(int i=0, j = start;i< count ;i++, j++){
				Keys[i] = leaf.Keys[j];
				Values[i] = leaf.Values[j];
			}	
			node.RemoveRange(start, count);
			Count += count;
		}

		public void RemoveRange(int index, int count){
			Algorithms.RemoveRange(Keys, index, count, Count);
			Algorithms.RemoveRange(Values, index, count, Count);
			Count -= count;
		}

		public bool Remove (TKey key, IComparer<TKey> comparer)
		{
			var index = Array.BinarySearch(Keys, 0, Count, key, comparer);
			if(index < 0) return false;
			RemoveAt(index); 
			return true;
		}

		public bool Add(TKey key, TValue value, IComparer<TKey> comparer, bool overwrite){
			var index = Array.BinarySearch(Keys, 0, Count, key, comparer);
			if(index >= 0){
				if(!overwrite)
					throw new Exception("Tried to insert duplicate key " + key);
				Keys[index] = key;
				Values[index] = value;
				return true;
			}

			if(Count == Keys.Length)
				return false;

			index = ~index;
			Algorithms.Insert(Keys, key, index, Count);
			Algorithms.Insert(Values, value,index, Count);
			Count++;
			return true;
		}

		public bool ContainsKey (TKey key, IComparer<TKey> comparer)
		{
			return Array.BinarySearch(Keys, 0, Count, key, comparer) >=0;
		}

		public int IndexOf (TKey key, IComparer<TKey> comparer)
		{
			var index = Array.BinarySearch(Keys, 0, Count, key, comparer);
			if(index < 0) index = ~index;
			return index;
		}

		public LeafSplit<TKey, TValue> Split(){
			Count /= 2;
			var half = Count;
			var right = new LeafNode<TKey,TValue>();
			Algorithms.Copy(Keys, half, right.Keys,0, half);
			Algorithms.Copy(Values, half, right.Values,0, half);
			right.Count = this.Count;
			return new LeafSplit<TKey,TValue>{
				Right = right,
				Middle = Keys[half-1] // right most key in left node 
			};
		}


	}

	public struct LeafSplit<TKey,TValue>{
		public LeafNode<TKey,TValue> Right;
		public TKey Middle;
	}


}

