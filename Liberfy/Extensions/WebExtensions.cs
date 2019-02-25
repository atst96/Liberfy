using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	internal static class WebExtensions
	{
		public static string DecodeHtml(this string text)
		{
			return WebUtility.HtmlDecode(text);
		}
	}
}
