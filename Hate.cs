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
		/// <summary>Determines whether or not a specific <see cref="Type"/> is convertable to another <see cref="Type"/>.</summary>
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

		/// <summary>Discovers the number of elements in an IEnumerable.</summary>
		public static int Count<T>(this IEnumerable<T> set)
		{
			int result = 0;
			IEnumerator<T> enumerator = set.GetEnumerator();
			while (enumerator.MoveNext()) result++;

			return result;
		}

		/// <summary>Returns the item at a specified index in an IEnumerable.</summary>
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

		/// <summary>Returns the maximum index value for an IEnumerable.</summary>
		public static int Max<T>(this IEnumerable<T> set)
		{
			return set.Count() - 1;
		}

		static Random rng = new Random();

		/// <summary>Returns a random item from an IEnumerable.</summary>
		public static T RandomItem<T>(this IEnumerable<T> set)
		{
			return set.RandomItem(rng);
		}

		/// <summary>Returns a random item from an IEnumerable, using the specified <see cref="Random"/>.</summary>
		public static T RandomItem<T>(this IEnumerable<T> set, Random rng)
		{
			return set.ItemAt(rng.Next(set.Max()));
		}

		/// <summary>Returns the specified <see cref="string"/> with the first character capitalized.</summary>
		public static string Capitalize(this string input) =>
		input switch
		{
			null => throw new ArgumentNullException(nameof(input)),
			"" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
			_ => input.First().ToString().ToUpper() + input.Substring(1)
		};

		/// <summary>Returns <paramref name="input"/> with the substring at <paramref name="index"/> replaced by <paramref name="replace"/>.</summary>
		public static string ReplaceChar(this string input, int index, string replace) =>
			(index + replace.Length > input.Length || index < 0)
			? throw new ArgumentOutOfRangeException("index")
			: input.Remove(index, replace.Length).Insert(index, replace);


		/// <summary>Encodes an array of <see cref="bool"/>s into a byte.</summary>
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

		/// <summary>Returns the bit at space <paramref name="bitNumber"/> from <paramref name="b"/> as a <see cref="bool"/>.</summary>
		public static bool GetBit(byte b, int bitNumber)
		{
			return (b & (1 << bitNumber)) != 0;
		}

		/// <summary>Returns a <see cref="bool"/>[] representing each bit of <paramref name="b"/>.</summary>
		public static bool[] ByteToBoolArr(byte b)
		{
			bool[] ret = new bool[8];
			for (int i = 0; i < 8; i++)
			{
				ret[i] = GetBit(b, i);
			}
			return ret;
		}

		/// <summary>Returns the Levenschtien Distance between two <see cref="string"/>s.</summary>
		public static int Levenschtien(string left, string right)
		{
			int n = left.Length;
			int m = right.Length;
			int[,] d = new int[n + 1, m + 1];
			if (n == 0) return m;
			if (m == 0) return n;
			for (int i = 0; i <= n; d[i, 0] = i++)
				for (int j = 0; j <= m; d[0, j] = j++)
					for (int k = 1; k <= n; k++)
					{
						for (int l = 1; l <= m; l++)
						{
							int cost = (right[l - 1] == left[k - 1]) ? 0 : 1;
							d[k, l] = Math.Min(
								Math.Min(d[k - 1, l] + 1, d[k, l - 1] + 1),
								d[k - 1, l - 1] + cost);
						}
					}
			return d[n, m];
		}
	}

	/// <summary>Represents a fixed-size Queue.</summary>
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
