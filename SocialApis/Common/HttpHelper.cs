using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    internal static class HttpHelper
    {
        private static readonly HashSet<byte> _passingCharacters;

        static HttpHelper()
        {
            var characters = new HashSet<byte>
            {
                0x2D, // -
                0x2E, // .
                0x5F, // _
                0x7E, // ~
            };

            for (byte c = 0x30; c <= 0x39; ++c)
                characters.Add(c);

            for (byte c = 0x41; c <= 0x5A; ++c)
                characters.Add(c);

            for (byte c = 0x61; c <= 0x7A; ++c)
                characters.Add(c);

            _passingCharacters = characters;
        }

        private static readonly Encoding UTF8Encoding = new UTF8Encoding(false);

        public static string UrlEncode(string value)
        {
            // 参考: https://nyahoon.com/blog/1291

            var data = UTF8Encoding.GetBytes(value);
            var sb = new StringBuilder(data.Length * 3);

            foreach (byte c in data)
            {
                if (_passingCharacters.Contains(c))
                {
                    sb.Append((char)c);
                }
                else
                {
                    sb.Append('%').Append(c.ToString("X2"));
                }
            }

            return sb.ToString();
        }
    }
}
