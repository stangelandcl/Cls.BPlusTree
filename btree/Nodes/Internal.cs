using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace btree
{
    [DebuggerDisplay("Nodes={Nodes.Count} Keys={Keys.Count}")]
	public class Internal<TKey,TValue> : INode<TKey, TValue>, IEnumerable<INode<TKey,TValue>>
	{
		public Internal(){
			Keys = new List<TKey> ();
            Nodes = new List<INode<TKey, TValue>>();
		}

		public void AddRange(IEnumerable<INode<TKey,TValue>> nodes, IComparer<TKey> comparer){
			foreach (var node in nodes)
				Add (node, comparer);
		}

		public void AddRange(INode<TKey,TValue> node, IComparer<TKey> comparer){
			AddRange (((Internal<TKey,TValue>)node).AsEnumerable(), comparer);
		}

		public void AddFromLeft(INode<TKey,TValue> node, IComparer<TKey> comparer){
			AddRange (((Internal<TKey,TValue>)node).TakeLeft (), comparer);
		}
		public void AddFromRight(INode<TKey,TValue> node, IComparer<TKey> comparer){
			AddRange (((Internal<TKey,TValue>)node).TakeRight (), comparer);
		}

		public bool Add(INode<TKey,TValue> node, IComparer<TKey> comparer){
			var index = Keys.BinarySearch (node.Keys [0], comparer);
			if (index >= 0) {
				Keys [index] = node.Keys [0];
				Nodes [index] = node;
				return true;
			}
			
			if(Keys.Count == Constants.NodeSize) return false;
			index = ~index;

			Keys.Insert (index, node.Keys [0]);
			Nodes.Insert (index, node);
			return true;
		}

		public bool IsFull{ get { return Keys.Count == Constants.NodeSize; } }

		internal NodeIndex<TKey, TValue> GetNode(TKey key, IComparer<TKey> comparer)
        {
			var index = Keys.BinarySearch (key, comparer);
			if (index < 0) {
				index = ~index;              
				if(index > 0) index --;
			}           
            return new NodeIndex<TKey, TValue> { Index = index, Node = Nodes[index] };
		}

        internal void UpdateNode(int index, TKey key, IComparer<TKey> comparer)
        {
            if (comparer.Compare(key, Keys[index]) < 0)
                Keys[index] = key;
        }
   

		public INode<TKey,TValue> Split(){
			var right = new Internal<TKey,TValue> ();
			var count = Constants.NodeSize / 2;
			right.Keys.AddRange (Keys.GetRange (count, count));
			Keys.RemoveRange (count, count);
			right.Nodes.AddRange (Nodes.GetRange (count, count));
			Nodes.RemoveRange (count, count);
			return right;
		}

		public int Count { get { return Keys.Count; } }
		public List<TKey> Keys {get; private set;}
        public List<INode<TKey, TValue>> Nodes { get; private set; }

		public INode<TKey,TValue> Left(int index){
            if (--index < 0) return null;
            return Nodes[index];			
		}

		public INode<TKey,TValue> Right(int index){
            if (++index == Count) return null;
            return Nodes[index];
		}

		public void Update(int index, INode<TKey,TValue> node){			
			Keys [index] = node.Keys [0];
		}

		public void Remove(int index){			
			Keys.RemoveAt (index);
			Nodes.RemoveAt (index);
		}

		public INode<TKey,TValue>[] TakeLeft(){
            var count = Constants.TakeCount(this);
			var items = new INode<TKey,TValue>[count];
			for (int i=0; i<items.Length; i++)
				items [i] = Nodes [i];
			Keys.RemoveRange (0, count);
			Nodes.RemoveRange (0, count);
			return items;
		}
		public INode<TKey,TValue>[] TakeRight(){
            var count = Constants.TakeCount(this);
			var items = new INode<TKey,TValue>[count];
			for(int i=0;i<items.Length;i++){
				var x = Keys.Count - count + i;
				items [i] = Nodes [x];
			}
			Keys.RemoveRange (Keys.Count - count, count);
			Nodes.RemoveRange (Nodes.Count - count, count);
			return items;
		}

        public void Replace(int index, INode<TKey, TValue> newNode)
        {            
            Nodes[index] = newNode;
            Keys[index] = newNode.Keys[0];
        }        


		#region IEnumerable implementation

		public IEnumerator<INode<TKey, TValue>> GetEnumerator ()
		{
			return Nodes.GetEnumerator ();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		#endregion
	}

    struct NodeIndex<TKey, TValue>
    {
        public int Index;
        public INode<TKey, TValue> Node;
    }
}

