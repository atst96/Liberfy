using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	static class MathEx
	{
		public static bool IsWithin(int value, int min, int max)
		{
			return min <= value && value <= max;
		}

		public static int RoundRange(int value, int min, int max)
		{
			return Math.Min(Math.Max(value, min), max);
		}
	}
}
