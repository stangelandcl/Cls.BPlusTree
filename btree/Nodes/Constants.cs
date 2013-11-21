using System;

namespace BTree
{
	public static class Constants
	{
		public const int NodeSize = 256;
		public const int MinNodeSize = NodeSize / 2;

        public static int TakeCount<TKey, TValue>(INode<TKey, TValue> node)
        {            
            return Math.Max((node.Count - Constants.MinNodeSize) / 2, 1);       
        }
	}
}

