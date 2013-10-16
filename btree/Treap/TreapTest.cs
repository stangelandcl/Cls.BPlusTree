using System;
using NUnit.Framework;
using System.Diagnostics;
using System.Linq;

namespace treap
{
	public class TreapTest
	{
		[Test]
		public static void TestPerformance(){
			const int Count = 1*1000*1000;
			var rand = new Random(1);
			var w = Stopwatch.StartNew();
			for(int i=0;i<Count;i++)
				rand.Next();
			Console.WriteLine("rand " + w.Elapsed);

			var items = Enumerable.Range(0, Count).Select(n=>rand.Next()).ToArray();
			var treap = new Treap<int>();
			w = Stopwatch.StartNew();
			foreach(var item in items)
				treap.Add(item);
			Console.WriteLine("insert " + w.Elapsed);

			w = Stopwatch.StartNew();
			foreach(var item in items)
				treap.Contains(item);
			Console.WriteLine("lookup " + w.Elapsed);

			w = Stopwatch.StartNew();
			foreach(var item in items){
				int v;
				treap.TryGetValue(item, out v);
			}
			Console.WriteLine("tryget " + w.Elapsed);

			w = Stopwatch.StartNew();
			foreach(var item in items)
				treap.Remove(item);
			Console.WriteLine("remove " + w.Elapsed);

			w = Stopwatch.StartNew();
			foreach(var item in items)
			{ //
			}
			Console.WriteLine("iterate " + w.Elapsed);

		}

		[Test]
		public static void Test(){
			const int Count = 1*1000;

			var treap = new Treap<int>();
			Assert.AreEqual(treap.Count, 0);

			var rand = new Random(1);
			var items = Enumerable.Range(0, Count *2).Select(n=> rand.Next()).Distinct().Take(Count).ToArray();


			for(int i=0;i<items.Length;i++){
				treap.Add(items[i]);
				Assert.AreEqual(i+1, treap.Count);
				for(int j= 0;j<=i;j++)
					Assert.IsTrue(treap.Contains(items[j]));
			}
			Assert.LessOrEqual(treap.Count, Count);

			var x = treap.First();
			int count =1;
			foreach(var item in treap.Skip(1)){
				Assert.Greater(item, x);
				x = item;
				count++;
			}
			Assert.AreEqual(count, treap.Count);

			foreach(var item in items){
				int v;
				Assert.IsTrue(treap.TryGetValue(item, out v));
				Assert.AreEqual(item, v);
			}

			count = treap.Count;
			for(int i=0;i<items.Length;i++){
				treap.Remove(items[i]);
				count--;

				Assert.AreEqual(count, treap.Count);
				for(int j= 0;j<=i;j++)
					Assert.IsFalse(treap.Contains(items[j]));
			}

		}
	}
}

