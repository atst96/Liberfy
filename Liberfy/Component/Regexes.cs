using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Liberfy
{
	static class Regexes
	{
		internal static readonly Regex ATagSource = new Regex("<a\\s+href=\"(?<url>.+?)\".*?>(?<name>.+?)</a>", RegexOptions.Compiled);
	}
}
