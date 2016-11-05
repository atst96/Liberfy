using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	public static class Extensions
	{
		public static T CastOrDefault<T>(this object obj)
		{
			return obj is T ? (T)obj : default(T);
		}
	}
}
