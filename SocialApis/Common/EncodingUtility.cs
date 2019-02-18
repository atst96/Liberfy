using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    internal static class EncodingUtility
    {
        public static Encoding ASCII { get; } = new ASCIIEncoding();
        public static Encoding UTF8 { get; } = new UTF8Encoding(false);
    }
}
