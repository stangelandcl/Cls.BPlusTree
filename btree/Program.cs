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
			var rand = new Random (1);
			var items = Enumerable.Range (0, 3000000).Select (n => rand.Next ()).Distinct ().ToArray ();

			TestAdd (tree, items);
			var set = new HashSet<int> (items);
            int i = 0;
            
			foreach (var item in items) {
				tree.Remove (item);
				set.Remove (item);
              
                //if (tree.Count() != set.Count())
                //    throw new Exception("Count mismatch at i=" + i);
                //if(++i % 10001 == 0)
                //Verify (tree);
                //AssertEqual(tree, set);
                i++;
                if (i % 10001 == 0 )
                {
                    Console.WriteLine(i);
                    //AssertEqual(tree, set);
                    //if (!tree.Verify())
                    //    throw new Exception("Invalid tree at " + i);
                   // Verify(tree);
                  
                }
			}
            tree.Verify();
		}

        static void AssertEqual(BTree<int, int> tree, HashSet<int> set)
        {
            var s2 = new HashSet<int>(set);
            foreach (var t in tree)            
                if (!s2.Remove(t.Key))
                    throw new Exception("Missing " + t.Key);
            if (s2.Count != 0)
                throw new Exception("remove failed " + s2.Count);
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
            tree.Verify();
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
