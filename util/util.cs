using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace util
{
	public static class prototype
	{
		public static string repeat(this string str, int count)
		{
			StringBuilder sb = new StringBuilder(str.Length * count);
			for (int i = 0; i < count; i += 1)
				sb.Append(str);

			return sb.ToString();
		}

		/// <summary>
		/// Finds the “minimum” element in the list based on the given comparator.
		/// If <c>splice</c> is true, the element is removed from the list.
		/// </summary>
		public static T MinMax<T>(this List<T> list, Func<T, T, int> cmp, bool splice = false)
		{
			if (list == null || list.Count == 0)
			{
				console.log("count: 0, minMax couldn't apply");
				return default(T);
			}
			int index = 0;
			T min = list[0];
			for (int i = 0; i < list.Count; i++)
			{
				T curr = list[i];
				if (cmp(curr, min) < 0)
				{
					min = curr;
					index = i;
				}
			}
			if (splice)
			{
				list.RemoveAt(index);
			}
			return min;
		}

		/// <summary>
		/// Traverses nested IList objects using an array of indexes.
		/// The traversal is done in reverse order (like the JS version).
		/// For example, given a nested list structure, calling GT with [i, j]
		/// returns the element at: nestedList[j][i]
		/// </summary>
		public static object GT(this IList list, int[] depthIndexes)
		{
			try
			{
				object curr = list;
				for (int i = depthIndexes.Length - 1; i >= 0; i--)
				{
					if (!(curr is IList currList) || currList.Count == 0 || currList[0] == null)
					{
						console.log($"null @depth-level: {i}");
						return null;
					}
					curr = currList[depthIndexes[i]];
				}
				return curr;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(
					$"GT() error: either depth-level-indexes are out of bounds or format is incorrect. Depth levels: {string.Join(", ", depthIndexes)}");
				return null;
			}
		}

		/// <summary>
		/// Traverses nested IList objects using depthIndexes (in reverse order)
		/// and sets the element at the final level (using the first index in the array)
		/// to the given value.
		/// </summary>
		public static void ST(this IList list, int[] depthIndexes, object val)
		{
			try
			{
				object curr = list;
				// Traverse all but the last “level”
				for (int i = depthIndexes.Length - 1; i >= 1; i--)
				{
					if (!(curr is IList currList) || currList.Count == 0 || currList[0] == null)
					{
						console.log($"null @depth-level: {i}");
						return;
					}
					curr = currList[depthIndexes[i]];
				}
				if (curr is IList finalList)
				{
					finalList[depthIndexes[0]] = val;
				}
				else
				{
					Console.Error.WriteLine("ST() error: final element is not a list");
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(
					$"ST() error: either depth-level-indexes are out of bounds or format is incorrect. Depth levels: {string.Join(", ", depthIndexes)}");
			}
		}

		/// <summary>
		/// Returns the element counting from the end of the list.
		/// For example, gl(0) returns the last element, gl(1) returns the one before last.
		/// </summary>
		public static T GL<T>(this List<T> list, int index)
		{
			if (index > list.Count - 1)
				console.log($"{index} > {list.Count} length of list");
			return list[list.Count - 1 - index];
		}

		/// <summary>
		/// (Optional) An extension that wraps List.Insert.
		/// List<T> already has an Insert method.
		/// </summary>
		public static void insert<T>(this List<T> list, int index, T val)
		{
			list.Insert(index, val);
		}

		/// <summary>
		/// Group the elements by a key function.
		/// If asMap is true, returns a Dictionary mapping keys to lists.
		/// Otherwise, returns a List of UnqGroup objects (each having Key and List properties).
		/// </summary>
		public static object UNQ<T, TKey>(this IEnumerable<T> source, Func<T, TKey> key_fn, bool as_map = false)
		{
			var groups = source.GroupBy(key_fn).ToDictionary(g => g.Key, g => g.ToList());
			if (as_map)
				return groups;
			else
				return groups.Select(g => new LIST<TKey, T>
				{
					Key = g.Key,
					List = g.Value
				}).ToList();
		}

		/// <summary>
		/// Helper class used by UNQ to represent a group.
		/// </summary>
		public class LIST<TKey, T>
		{
			public TKey Key { get; set; }
			public List<T> List { get; set; }
		}
	}


	public static class U
	{
		// floor
		public static int floor(float x)
		{
			if (x < 0f) return (int)x - 1;
			return (int)x;
		}


		// get next pseudo random: gnpr
		public class xoro128
		{
			public ulong[] seed = new ulong[2];
			public xoro128(ulong initial_seed = 0)
			{
				this.seed[0] = initial_seed << 3;
				this.seed[1] = initial_seed >> 11;
			}

			// get pseudo random
			public int get_pr(int min = 0, int max = 100)
			{
				ulong rl(ulong val, int bits) => (val << bits) | (val >> (64 - bits));

				ulong s0 = this.seed[0]; ulong s1 = this.seed[1];
				s1 ^= s0;
				this.seed[0] = rl(s0, 55) ^ s1 ^ (s1 << 14);
				this.seed[1] = rl(s1, 36);
				ulong result = s0 + s1; 
				//console.log(s0, s1, result);
				return (int)((ulong)min + (result % (ulong)(max - min + 1)));
			}
		}

		// delay
		public static IEnumerator wait(int ms)
		{
			//console.log(ms * 0.001f);
			yield return new WaitForSeconds(ms * 0.001f);
		}
		public static Task delay(int ms)
		{
			return Task.Delay(ms);
		}
	}

	public static class ITER
	{

		public static int iter = 0;
		public static bool iter_inc(int max = 100)
		{
			if (iter >= max) { console.log($"iter > {max}"); return true; }
			iter += 1;
			return false;
		}
	}


	// ad >>
	public static class console
	{
		public static void log(params object[] MSSG)
		{
			// m?: is null, ??: catch previous instruction  
			string combined = string.Join("__", MSSG.Select(m => m?.ToString() ?? "no str conv"));
			if (log_mode == "txt")
			{
				string timestamp = $"--{System.DateTime.Now:HH:mm:ss}";
				log_txt_str += $"{combined}\n{" ".repeat(45)}{timestamp} \n\n";
			}
			else
				Debug.Log(combined);
		}

		public static void error(params object[] MSSG)
		{
			// m?: is null, ??: catch previous instruction  
			string combined = string.Join("__", MSSG.Select(m => m?.ToString() ?? "no str conv"));
			if (log_mode == "txt")
			{
				string timestamp = $"--{console.time}";
				log_txt_str += $"error: {combined}\n{" ".repeat(45)}{timestamp} \n\n";
			}
			else
				Debug.Log(combined);
		}

		public static string time
		{
			get { return $"{System.DateTime.Now:mm:ss.fff}"; }
		}

		public static string log_mode = "log";
		static string log_txt_str = "";
		public static void LOG_txt()
		{
			if (log_mode == "txt")
			{
				string loc_file = Application.dataPath + "/LOG/LOG.txt";
				System.IO.File.WriteAllText(loc_file, log_txt_str);
			}
		}
	}
	// << ad
}