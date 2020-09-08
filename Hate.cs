using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hate
{
	public static class Tools
	{
		public static bool IsConvertable(Type fromType, Type toType)
		{
			try
			{
				Expression.Convert(Expression.Parameter(fromType, null), toType);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public static int Count<T>(this IEnumerable<T> set)
		{
			int result = 0;
			IEnumerator<T> enumerator = set.GetEnumerator();
			while (enumerator.MoveNext()) result++;

			return result;
		}

		public static T ItemAt<T>(this IEnumerable<T> set, int index)
		{
			int i = 0;
			IEnumerator<T> enumerator = set.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (i == index)
				{
					return enumerator.Current;
				}
				i++;
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public static int Max<T>(this IEnumerable<T> set)
		{
			return set.Count() - 1;
		}

		static Random rng = new Random();

		public static T RandomItem<T>(this IEnumerable<T> set)
		{
			return set.RandomItem(rng);
		}

		public static T RandomItem<T>(this IEnumerable<T> set, Random rng)
		{
			return set.ItemAt(rng.Next(set.Max()));
		}

		public static string Capitalize(this string input) =>
		input switch
		{
			null => throw new ArgumentNullException(nameof(input)),
			"" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
			_ => input.First().ToString().ToUpper() + input.Substring(1)
		};

		public static string ReplaceChar(this string input, int index, string replace) =>
			(index + replace.Length > input.Length || index < 0)
			? throw new ArgumentOutOfRangeException("index")
			: input.Remove(index, replace.Length).Insert(index, replace);

		public static byte EncodeBool(bool[] arr)
		{
			byte val = 0;
			foreach (bool b in arr)
			{
				val <<= 1;
				if (b) val |= 1;
			}
			return val;
		}

		public static bool GetBit(byte b, int bitNumber)
		{
			return (b & (1 << bitNumber)) != 0;
		}

		public static bool[] ByteToBoolArr(byte b)
		{
			bool[] ret = new bool[8];
			for (int i = 0; i < 8; i++)
			{
				ret[i] = GetBit(b, i);
			}
			return ret;
		}

		public static int Levenschtien(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];
			if (n == 0) return m;
			if (m == 0) return n;
			for (int i = 0; i <= n; d[i, 0] = i++)
				for (int j = 0; j <= m; d[0, j] = j++)
					for (int k = 1; k <= n; k++)
					{
						for (int l = 1; l <= m; l++)
						{
							int cost = (t[l - 1] == s[k - 1]) ? 0 : 1;
							d[k, l] = Math.Min(
								Math.Min(d[k - 1, l] + 1, d[k, l - 1] + 1),
								d[k - 1, l - 1] + cost);
						}
					}
			return d[n, m];
		}
	}

	public class FQueue<T> : Queue<T>
	{
		readonly uint maxEntries;

		public FQueue() : base(1)
		{
			maxEntries = 1;
		}

		public FQueue(uint maxEntries) : base((int)maxEntries)
		{
			this.maxEntries = maxEntries > 0 ? this.maxEntries : 1;
		}

		public FQueue(IEnumerable<T> collection) : base(collection)
		{
			maxEntries = (uint)collection.Count();
		}

		public List<T> Read()
		{
			return this.ToList();
		}

		public new void Enqueue(T item)
		{
			while (Count + 1 > maxEntries)
			{
				Dequeue();
			}
			base.Enqueue(item);
		}
	}
}
