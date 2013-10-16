using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace treap
{
	public static class Algorithms
	{
		public static int BinarySearch<T, U>(List<T> items, U key, IComparer<U, T> comparer){

			int low = 0;
			int high = items.Count -1;
			while(low <= high){
				var mid = low + (high - low)/2;
				var x   = items[mid];
				var c   = comparer.Compare(key, x);
				if(c < 0)      high = mid -1;
				else if(c > 0) low = mid +1;
				else           return mid;
			}
			return ~low;
		}

		public static void Insert<T>(T[] array, T item, int index, int count){
			if(index < count)			
				Array.Copy(array,index, array, index+1, count - index);
			array[index] = item;
		}

		[Conditional("DEBUG")]
		static void Clear<T>(T[] keys, int index){			
			keys[index] = default(T);
		}

		public static void RemoveAt <T>(T[] keys, int index, int count)
		{
			Array.Copy(keys,index + 1, keys, index, count - index -1);
			Clear(keys, count -1);
		}
	}
}

