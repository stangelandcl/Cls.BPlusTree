using System;
using NUnit.Framework;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace treap
{
	public class BTreeTest
	{
//		[Test]
//		public static void TestPerformance(){
//			const int Count = 1*1000*1000;
//			var rand = new Random(1);
//			var w = Stopwatch.StartNew();
//			for(int i=0;i<Count;i++)
//				rand.Next();
//			Console.WriteLine("rand " + w.Elapsed);
//
//			var items = Enumerable.Range(0, Count).Select(n=>rand.Next()).ToArray();
//			var treap = new BTree<int>();
//			w = Stopwatch.StartNew();
//			foreach(var item in items)
//				treap.Add(item);
//			Console.WriteLine("insert " + w.Elapsed);
//
//			w = Stopwatch.StartNew();
//			foreach(var item in items)
//				treap.Contains(item);
//			Console.WriteLine("lookup " + w.Elapsed);
//
//			w = Stopwatch.StartNew();
//			foreach(var item in items){
//				int v;
//				treap.TryGetValue(item, out v);
//			}
//			Console.WriteLine("tryget " + w.Elapsed);
//
//			w = Stopwatch.StartNew();
//			foreach(var item in items)
//				treap.Remove(item);
//			Console.WriteLine("remove " + w.Elapsed);
//
//			w = Stopwatch.StartNew();
//			foreach(var item in items)
//			{ //
//			}
//			Console.WriteLine("iterate " + w.Elapsed);
//
//		}

		[Test]
		public static void Test(){
			const int Count = 1000;

			var tree = new BTree<int,int>();
			Assert.AreEqual(tree.Count, 0);

			var rand = new Random(1);
			var items = Enumerable.Range(0, Count *2).Select(n=> rand.Next()).Distinct().Take(Count).ToArray();

//			var sw  = Stopwatch.StartNew();
//			for(int i=0;i<items.Length;i++)
//				tree[items[i]] = i;
//			Console.WriteLine("insert " + Count + " " + sw.Elapsed);
//
//			sw  = Stopwatch.StartNew();
//			var x = tree.First();
//			foreach(var item in tree.Skip(1)){
//				if(x.Key >= item.Key)
//					throw new Exception("Out of order " + item.Key);
//				x = item;
//			}
//			Console.WriteLine("iterate " + Count + " " + sw.Elapsed);
//
//			var map = new Dictionary<int,int>();
//			 sw  = Stopwatch.StartNew();
//			for(int i=0;i<items.Length;i++)
//				map[items[i]] = i;
//			Console.WriteLine("map " + Count + " " + sw.Elapsed);


			tree.Clear();
			Console.WriteLine("running " + Count + " iterations");
			for(int i=0;i<items.Length;i++){
				if(i % 1000 == 0)
					Console.WriteLine("iteration " + i);
				tree[items[i]] = items[i];
				//Assert.AreEqual(i+1, tree.Count);		
				var seq = 1;
				if(i > 5000)
					seq = 100100;
				for(int j= 0;j<=i;j+=seq)
					Assert.IsTrue(tree.ContainsKey(items[j]));

				//if((i % 100) == 0)
					//Console.WriteLine("At index " + i);
			}
//			Assert.LessOrEqual(treap.Count, Count);
//
//			foreach(var item in items){
//				int v;
//				Assert.IsTrue(treap.TryGetValue(item, out v));
//				Assert.AreEqual(item, v);
//			}
//
//			
//			var x = treap.First();
//			int count =1;
//			foreach(var item in treap.Skip(1)){
//				Assert.Greater(item, x);
//				x = item;
//				count++;
//			}
//			Assert.AreEqual(count, treap.Count);
//
//			count = treap.Count;
			for(int i=0;i<items.Length;i++){
				tree.Remove(items[i]);
				//count--;

				//Assert.AreEqual(count, tree.Count);
				for(int j= 0;j<=i;j++)
					Assert.IsFalse(tree.ContainsKey(items[j]));
			}

		}
	}
}

