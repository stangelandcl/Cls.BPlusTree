using System;
using System.Linq;
using System.Collections.Generic;

namespace btree
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var tree = new BTree<int,int> ();
			var rand = new Random ();
			var items = Enumerable.Range (0, 129).Select (n => rand.Next ()).Distinct ().ToArray ();

			TestAdd (tree, items);
			var set = new HashSet<int> (items);
			foreach (var item in items) {
				tree.Remove (item);
				set.Remove (item);
				Verify (tree);
			}
		}

		static void TestAdd (BTree<int, int> tree, int[] items)
		{
			int i = 0;
			foreach (var item in items) {
				tree.Add (item, item);
				if (++i % 1000 == 0) {
					Console.WriteLine (i);
					//Verify (tree);
				}
			}
			Verify (tree);
		}

		static void Verify (BTree<int, int> tree)
		{
			int? i = null;
			foreach (var item in tree.Select (n => n.Key)) {
				if (i.HasValue)
					if (item <= i.Value)
						throw new Exception ("dup or out of order " + item + " and " + i.Value);
				i = item;
			}
		}
	}
}
