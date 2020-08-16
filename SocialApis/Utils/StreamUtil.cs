using System.IO;
using System.Threading.Tasks;

namespace SocialApis.Utils
{
    internal static class StreamUtil
    {
        public static string ReadToEnd(Stream stream)
        {
            using var reader = new StreamReader(stream, EncodingUtil.UTF8);

            return reader.ReadToEnd();
        }

        public static async Task<string> ReadToEndAsync(this Stream stream)
        {
            using var reader = new StreamReader(stream, EncodingUtil.UTF8);

            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
