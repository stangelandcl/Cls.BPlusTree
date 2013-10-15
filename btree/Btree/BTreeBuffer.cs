using System;
using System.Collections.Generic;
using System.Linq;

namespace treap
{
	public class BTreeBuffer{
		public const int MaxSize = 512;
	}

	public struct BTreeBuffer<T>
	{
		public BTreeBuffer(int a){
			Buffer = new List<T>(BTreeBuffer.MaxSize);
		}

		public List<T> Buffer;
		public int Index (T item, IComparer<T> comparer)
		{
			return Buffer.BinarySearch(item, comparer);
		}

		public void TakeAll (List<T> items, IComparer<T> comparer)
		{
			Merge(items, Buffer, comparer);
			//items.AddRange(buffer);
			Buffer.Clear();	
		}

//		public bool Add(T item, IComparer<T> comparer){
//			int index = buffer.BinarySearch(item, comparer);
//			if(index >= 0){
//				buffer[index] = item;
//				return true;
//			}
//			index = ~index;
//			if(index == MaxSize)
//				return false;
//			buffer.Insert(index, item);
//			return true;
//		}

		void RemoveExisting (List<T> items, IComparer<T> comparer)
		{	
			//if(items.Count < buffer.Count){
				foreach(var item in items){
					int i = Buffer.BinarySearch (item,comparer);
					if (i >= 0)
						Buffer.RemoveAt (i);				
				}
//			}else{
//				for(int j= buffer.Count-1;j>=0;j--){
//					var item = buffer[j];
//					int i = items.BinarySearch (item,comparer);
//					if (i >= 0)
//						buffer.RemoveAt (j);				
//				}
//			}
		}

		static void Fill (List<T> a, int ai, List<T> result)
		{
			if (ai == 0)
				result.AddRange (a);
			else
				result.AddRange(a.GetRange(ai, a.Count - ai));
				//for (int i = ai; i < a.Count; i++)
				//	result.Add (a [i]);
		}

		static void Merge(List<T> a, List<T> b, IComparer<T> comparer){
			if(b.Count == 0)
				return;
			if(a.Count == 0){
//				if(b.Count <= MaxSize)
//					buffer = b;
				//				else 
				a.AddRange(b);
				return;
			}
	
			var final = new List<T>(a.Count + b.Count);
			int ai = 0, bi = 0;
			var ax = a[ai]; var bx = b[bi];
			while(true){
				int c = comparer.Compare(ax, bx);
				if(c < 0){
					final.Add(ax);							
					if(++ai == a.Count)
					{
						Fill(b, bi, final);
						break;
					}
					ax = a[ai];
				}else if(c > 0){
					final.Add(bx);
					if(++bi == b.Count){
						Fill(a, ai, final);
						break;
					}
					bx = b[bi];
				}else{
					final.Add(bx);
					if(++bi == b.Count){
						Fill (a, ++ai, final);
						break;
					}
										
					if(++ai == a.Count)
					{
						Fill(b, bi, final);
						break;
					}
					ax = a[ai];
					bx = b[bi];
				}
			}
			a.Clear();
			a.AddRange(final);
		}

		/// <summary>
		/// Returns 0 = added
		/// 1 = go right
		/// -1 = go left
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="separator">Separator.</param>
		/// <param name="comparer">Comparer.</param>
		public int Add(List<T> items, ref T item, IComparer<T> comparer){
			//RemoveExisting (items, comparer); // because the 'items' list may have newer versions of the same key
			int index;
			if(items.Count == 1){
				if(comparer.Compare(item, items[0]) == 0){
					item = items[0];
				}else{
					index = Buffer.BinarySearch(items[0], comparer);
					if(index < 0) index = ~index;
					Buffer.Insert(index, items[0]);
				}
			}else{
				Merge(Buffer, items, comparer);
			}
			if(Buffer.Count <= BTreeBuffer.MaxSize)
				return 0;

			items.Clear();
			index = Buffer.BinarySearch(item, comparer);
			if(index >= 0){
				item = Buffer[index];
				Buffer.RemoveAt(index);
			}else index = ~index;
			int rightCount = Buffer.Count -index;
			if(index < rightCount){
				// move large group down the chain.
				//Merge(items, buffer.GetRange(index, rightCount), comparer);			
				items.AddRange(Buffer.GetRange(index, rightCount));
				Buffer.RemoveRange(index, rightCount);
				return 1;
			}else{
				//Merge(items, buffer.GetRange(0, index), comparer);
				items.AddRange(Buffer.GetRange(0, index));
				Buffer.RemoveRange(0, index);
				return -1;
			}
		}
	}
}

