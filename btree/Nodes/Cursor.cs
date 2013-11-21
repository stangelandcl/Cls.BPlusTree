using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTree
{
    class BTreeCursor<TKey,TValue> : ISortedDictionaryCursor<TKey,TValue>
    {
        public BTreeCursor(BTreeDictionary<TKey, TValue> tree, KeyPosition<TKey, TValue> position)
        {
            this.tree = tree;
            this.position = position;
        }

        KeyPosition<TKey, TValue> position;
        BTreeDictionary<TKey, TValue> tree;

        public bool MoveTo(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Next()
        {
            if (position.Count == 0)
                return false;

            var top = position.Pop();
            top.Index += 1;
            if (top.Index < top.Node.Keys.Count)
            {
                position.Push(top);
                return true;
            }

            throw new NotImplementedException();
        }

        public bool Previous()
        {
            throw new NotImplementedException();
        }

        public void First()
        {
            throw new NotImplementedException();
        }

        public void Last()
        {
            throw new NotImplementedException();
        }
    }
}
