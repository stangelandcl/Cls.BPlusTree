using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace btree
{
    class KeyPosition<TKey, TValue> : Stack<KeyIndex<TKey, TValue>>
    {       
    }

    static class KeyIndex
    {
        public static KeyIndex<TKey, TValue> New<TKey, TValue>(int index, INode<TKey, TValue> node)
        {
            return new KeyIndex<TKey, TValue> { Index = index, Node = node };
        }
    }

    struct KeyIndex<TKey,TValue>
    {
        public int Index;
        public INode<TKey, TValue> Node;
    }
}
