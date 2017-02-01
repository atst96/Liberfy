using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	static class Info
	{
		public static OSInfo OS { get; } = new OSInfo();

		public static AppInfo App { get; } = new AppInfo();
	}
}
