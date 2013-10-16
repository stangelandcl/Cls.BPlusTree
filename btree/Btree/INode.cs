using System;

namespace treap
{
	public interface INode<TKey, TValue>
	{
		void RemoveAt(int index);
		void AddRight(INode<TKey,TValue> node, int count);
		void AddLeft(INode<TKey,TValue> node, int count);
		int Count {get; set;}
		TKey[] Keys {get;}
		void RemoveRange(int index, int count);
	}

	public static class Constants{
		public const int NodeSize = 128;
		public const int MinimumSize = NodeSize / 2;
	}
}

