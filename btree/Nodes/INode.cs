using System;
using System.Collections.Generic;

namespace btree
{
	public interface INode<TKey, TValue>
	{
		List<TKey> Keys { get; }
		INode<TKey,TValue> Split();
		int Count {get;}
		void AddRange(INode<TKey,TValue> node, IComparer<TKey> comparer);
		void AddFromLeft(INode<TKey,TValue> node, IComparer<TKey> comparer);
		void AddFromRight(INode<TKey,TValue> node, IComparer<TKey> comparer);
	}
}

