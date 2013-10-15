using System;
using System.Collections.Generic;

namespace treap
{
	public class TreapNode<T>{
		public TreapNode(T v, int p){
			Value=v;
			Priority = p;
		}

		public T Value {get;set;}
		public int Priority {get;set;}
		public TreapNode<T> Left;
		public TreapNode<T> Right;
	}

	public class TreapIterator<T>{
		public TreapNode<T> Node;
		internal Stack<TreapNode<T>> stack;
		public bool Next(){
			if(stack.Count == 0)
				return false;
			Node = stack.Pop();
			return true;
		}
		public bool Previous(){return false;}
	}

	public interface IComparer<U, T>{
		int Compare(U x, T y);
	}
	public class LikeComparer<T> : IComparer<T, T>{
		public LikeComparer(IComparer<T> comparer){
			this.comparer = comparer;
		}
		IComparer<T> comparer;
		public int Compare(T x, T y){ 
			return comparer.Compare(x, y);
		}
	}
}

