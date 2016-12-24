using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MDDSharp
{
	public class MDD
	{
		public static MDD one = new MDD(Int32.MaxValue, null);
		public static MDD zero = new MDD(Int32.MaxValue - 1, null);
		
		class MDDComparer : IEqualityComparer<MDD>
		{
		    public bool Equals(MDD x, MDD y)
		    {
		        return x.var == y.var && x.childs.SequenceEqual(y.childs);
		    }
		    public int GetHashCode(MDD obj)
		    {
		        int hashcode = 0;
		        foreach (var t in obj.childs)
		        {
		            hashcode ^= t.GetHashCode();
		        }
		        hashcode ^= obj.var;
		        return hashcode;
		    }
		}
		
		static Dictionary<MDD, MDD> UniqueTable = new Dictionary<MDD, MDD>(1024,new MDDComparer());
		static Dictionary<Tuple<MDD, MDD, MDD>, MDD> ITECache = new Dictionary<Tuple<MDD, MDD, MDD>, MDD>();
		
		int var;
		IEnumerable<MDD> childs;
		
		private MDD(int var, IEnumerable<MDD> childs)
		{
			this.var = var;
			this.childs = childs;
		}
		
		public static MDD MakeNode(int var, IEnumerable<MDD> childs)
		{
			var first = childs.First();
			if(childs.Any(o => o != first))
			{
				var key = new MDD(var, childs);
				MDD ret;
				if(UniqueTable.TryGetValue(key, out ret))
				{
					return ret;
				}
				else
				{
					UniqueTable[key] = key;
					return key;
				}
			} 
			else
			{
				return childs.First();
			}
		}
		
		public MDD ITE(MDD g, MDD u, MDD v)
		{
			if(g == one)
			{
				return u;
			} 
			else if (g == zero)
			{
				return v;
			}
			else
			{
				MDD ret;
				if(ITECache.TryGetValue(new Tuple<MDD, MDD, MDD>(g, u, v), out ret))
				{
					return ret;
				}
				int z = Math.Min(g.var, Math.Min(u.var, v.var));
				var gc = TopCofactor(g, z);
		        var uc = TopCofactor(u, z);
		        var vc = TopCofactor(v, z);
		        var nodes = Zip(gc, uc, vc, (a, b, c) => ITE(a, b, c));
		        ret = MakeNode(z, nodes);
		        ITECache.Add(new Tuple<MDD, MDD, MDD>(g, u, v), ret);
		        return ret;
			}
		}
		
		private static IEnumerable<MDD> TopCofactor(MDD u, int i)
		{
			if(u == one || u == zero || i < u.var)
			{
				return Enumerable.Repeat(u, 100);
			}
			else
			{
				Debug.Assert(u.var == i);
				return u.childs;
			}
		}
		
		public MDD And(MDD v)
		{
			return ITE(this, v, zero);
		}
		
		public MDD Or(MDD v)
		{
			return ITE(this, one, v);
		}
		
		public MDD Not()
		{
			return ITE(this, zero, one);
		}
		
		private static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(
		    IEnumerable<TFirst> first,
		    IEnumerable<TSecond> second,
		    IEnumerable<TThird> third,
		    Func<TFirst, TSecond, TThird, TResult> resultSelector)
		{
		    using (var enum1 = first.GetEnumerator())
		    using (var enum2 = second.GetEnumerator())
		    using (var enum3 = third.GetEnumerator())
		    {
		        while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext())
		        {
		            yield return resultSelector(
		                enum1.Current,
		                enum2.Current,
		                enum3.Current);
		        }
		    }
		}
		
		public override string ToString()
		{
			if(this == zero) { 
				return "f"; 
			} else if (this == one) {
				return "t";
			}
			return "(" + this.var + " " + string.Join(" ", childs) + ")"; 
		}
		
		public Int32 Id { get { return GetHashCode(); } }
	}
}