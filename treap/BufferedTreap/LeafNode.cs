using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace treap
{
	public class LeafNode<TKey, TValue> : INode{
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

		public int Count;
		public TKey[] Keys;
		public TValue[] Values;

		public bool Remove (TKey key, IComparer<TKey> comparer)
		{
			var index = Array.BinarySearch(Keys, 0, Count, key, comparer);
			if(index < 0) return false;
			Algorithms.RemoveAt(Keys, index, Count);
			Algorithms.RemoveAt(Values, index, Count);
			Count--;
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
			Array.Copy(Keys, half, right.Keys,0, half);
			Array.Copy(Values, half, right.Values,0, half);
			right.Count = this.Count;
			return new LeafSplit<TKey,TValue>{
				Right = right,
				Middle = Keys[half-1]
			};
		}


	}

	public struct LeafSplit<TKey,TValue>{
		public LeafNode<TKey,TValue> Right;
		public TKey Middle;
	}


}

