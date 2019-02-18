using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    internal static class StreamUtility
    {
        public static string ReadToEnd(Stream stream)
        {
            using (var reader = new StreamReader(stream, EncodingUtility.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<string> ReadToEndAsync(this Stream stream)
        {
            using (var reader = new StreamReader(stream, EncodingUtility.UTF8))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
