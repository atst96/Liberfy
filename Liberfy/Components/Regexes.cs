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
        private static Regex _twitterSourceHtml;
        public static Regex TwitterSourceHtml => _twitterSourceHtml ?? (_twitterSourceHtml = new Regex("<a\\s+href=\"(?<url>.+?)\".*?>(?<name>.+?)</a>", RegexOptions.Compiled));
    }
}
