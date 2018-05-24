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

		public static int WithIn(int value, int min, int max)
		{
			return min > value ? min : (value > max ? max : value);
		}
	}
}
