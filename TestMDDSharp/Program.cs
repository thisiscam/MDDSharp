using System;
using MDDSharp;

namespace TestMDDSharp
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			MDD v0 = MDD.MakeNode(0, new MDD[]{MDD.zero, MDD.one});
			MDD nv0 = MDD.MakeNode(0, new MDD[]{MDD.one, MDD.zero});
			if(v0.Not() != nv0)
			{
				Console.WriteLine("Not failed {0}, {1}", v0.Not(), nv0);
			}
			
			MDD v1 = MDD.MakeNode(1, new MDD[]{MDD.one, MDD.zero});
			if(v1.And(v0).Not() != v1.Not().Or(v0.Not()))
			{
				Console.WriteLine("And/Or/Not failed {0}, {1}", v1.And(v0).Not(), v1.Not().Or(v0.Not()));
			}
			
			MDD v2 = MDD.MakeNode(2, new MDD[]{MDD.one, MDD.zero, MDD.one});
			
			Console.WriteLine("v0 & v1 & v2 = {0}", v0.And(v1.And(v2)));
		}
	}
}
