using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocialApis.Common
{
    public static class Regexen
    {
        internal static readonly Regex TwitterSourceHtml = new Regex("<a\\s+href=\"(?<url>.+?)\".*?>(?<name>.+?)</a>", RegexOptions.Compiled);
    }
}
