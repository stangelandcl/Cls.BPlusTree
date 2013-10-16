using System;
using NUnit.Framework;
using System.Linq;
using System.Diagnostics;

namespace treap
{
	[TestFixture]
	class MainClass
	{
		public static void Main (string[] args)
		{
			BTreeTest.Test();
		//	BTreeTest.TestPerformance();
		//	TestPerformance();
			//Test();
			//TestPerformance();
			Console.WriteLine("passed");
		}			
	}
}
