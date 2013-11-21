using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTree
{
    public interface ISortedDictionary<TKey,TValue> : IDictionary<TKey, TValue>
    {
        ISortedDictionaryCursor<TKey, TValue> Cursor();
    }

    public interface ISortedDictionaryCursor<TKey, TValue>
    {
        bool MoveTo(TKey key);
        bool Next();
        bool Previous();
        void First();
        void Last();
    }
}
