using System;
using System.Collections.Generic;
using System.Linq;

namespace treap
{	
	public class Treap<T> : ICollection<T>
	{
		public Treap () : this(Comparer<T>.Default){}
		public Treap (IComparer<T> comparer)
		{
			this.comparer = comparer;
			this.random = new Random(36243606);
			this.likeComparer = new LikeComparer<T>(comparer);
		}

		IComparer<T> comparer;
		TreapNode<T> root;
		Random random;
		LikeComparer<T> likeComparer;

		#region ICollection implementation	

		public void Add (T item)
		{
			Add(ref root, item);
		}

		void Add (ref TreapNode<T> node, T item)
		{
			if(node == null){
				node = new TreapNode<T>(item, random.Next());
				Count++;
				return;
			}

			var c = comparer.Compare (item, node.Value);
			if (c < 0){
				Add(ref node.Left, item);			
				if(node.Left.Priority > node.Priority){
					var x = node.Left;
					node.Left = x.Right;
					x.Right = node;
					node = x;
				}
			}
			else if(c > 0){
				Add(ref node.Right, item);
				if(node.Priority < node.Right.Priority){
					var x = node.Right;
					node.Right = x.Left;
					x.Left = node;
					node = x;
				}
			}else  node.Value = item;	
		}

		public void Clear ()
		{
			this.root = null;
			Count = 0;
		}

		public bool TryGetValue(T key, out T item){
			return TryGetValue(root, key, out item, likeComparer);
		}
			
		public bool TryGetValue<U>(U key, out T item, IComparer<U, T> comparer){
			return TryGetValue(root, key, out item, comparer);	
		}

		bool TryGetValue<U>(TreapNode<T> node, U key, out T item, IComparer<U, T> comparer){
			if(node == null){
				item = default(T);
				return false;
			}

			int c = comparer.Compare(key, node.Value);
			if(c < 0) return TryGetValue(node.Left, key, out item, comparer);
			if(c > 0) return TryGetValue(node.Right, key, out item, comparer);
			item = node.Value;
			return true;
		}

		public bool Contains (T item)
		{
			return Contains(root, item);
		}

		bool Contains(TreapNode<T> node, T item){
			if(node == null)
				return false;
			var c = comparer.Compare(item, node.Value);
			if(c < 0) return Contains(node.Left, item);
			if(c > 0) return Contains(node.Right, item);
			return true;
		}

		public void CopyTo (T[] array, int arrayIndex)
		{
			foreach(var item in this)
				array[arrayIndex++] = item;
		}

		static void Reorder(ref TreapNode<T> node, TreapNode<T> left, TreapNode<T> right){
			if(left == null){
				node = right;
				return;
			}
			if(right == null){
				node = left;
				return;
			}
			if(left.Priority > right.Priority){
				node = left;
				Reorder(ref node.Right, node.Right, right);
			}else{			
				node = right;
				Reorder(ref node.Left, left, node.Left);
			}
		}

		public bool Remove (T item)
		{
			return Remove(ref root, item);
		}

		bool Remove(ref TreapNode<T> node, T item){
			if(node == null)
				return false;
			var c = comparer.Compare(item, node.Value);
			if(c < 0) return Remove(ref node.Left, item);				
			if(c > 0) return Remove(ref node.Right, item);

			if(node.Left != null){
				if(node.Right != null) Reorder(ref node, node.Left, node.Right);
				else                   node = node.Left;
			}else                      node = node.Right;

			Count--;
			return true;
		}

		public int Count { get; private set;}
		public bool IsReadOnly { get { return false; } }

		#endregion

		#region IEnumerable implementation
		public IEnumerator<T> GetEnumerator ()
		{
			return Next(root).GetEnumerator();
		}

		IEnumerable<T> Next(TreapNode<T> node){
			if(node == null) yield break;
			foreach(var t in Next(node.Left))
				yield return t;
			yield return node.Value;
			foreach(var t in Next(node.Right))
				yield return t;
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

