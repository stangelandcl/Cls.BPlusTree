using System;

namespace btree
{
	public static class Constants
	{
		public const int NodeSize = 128;
		public const int MinNodeSize = NodeSize / 2;

        public static int TakeCount<TKey, TValue>(INode<TKey, TValue> node)
        {            
            return Math.Max((node.Count - Constants.MinNodeSize) / 2, 1);       
        }
	}
}

