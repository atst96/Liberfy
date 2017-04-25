using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			return obj is T ? (T)obj : default(T);
		}

		public static void DisposeAll<T>(this IEnumerable<T> collection) where T: IDisposable
		{
			foreach(var item in collection)
			{
				item.Dispose();
			}
		}
	}
}
