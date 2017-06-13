using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
	public static class Extensions
	{
		public static bool Contains(this string value, string find, StringComparison comparison)
		{
			return value?.IndexOf(find, comparison) >= 0;
		}

		public static T CastOrDefault<T>(this object obj)
		{
			return obj is T tObj ? tObj : default(T);
		}

		public static void DisposeAll<T>(this IEnumerable<T> collection) where T : IDisposable
		{
			foreach (var item in collection)
			{
				item.Dispose();
			}
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			if (action == null)
				return;

			foreach (var item in collection)
			{
				action(item);
			}
		}

		public static async void ForEach<T>(this IEnumerable<T> collection, Action<T> action, Dispatcher dispatcher)
		{
			if (action == null)
				return;

			Action d = delegate { };
			await dispatcher.InvokeAsync(() =>
			{
				foreach (var item in collection)
				{
					action(item);
				}
			});
		}

		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> collection)
		{
			var list = new List<T>(collection.Count());

			foreach (var item in collection)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}

			return list;
		}

		public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> collection)
		{
			foreach (var items in collection.Where(items => items != null))
			{
				foreach (var item in items.Where(item => item != null))
				{
					yield return item;
				}
			}

			yield break;
		}
	}
}
