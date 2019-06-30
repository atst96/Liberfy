using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy.Utilieis
{
    internal static class JsonUtility
    {
        private static IJsonFormatterResolver _jsonFormatterResolver { get; } = Utf8Json.Resolvers.StandardResolver.AllowPrivate;

        public static Task<T> DeserializeAsync<T>(Stream stream)
        {
            return JsonSerializer.DeserializeAsync<T>(stream, _jsonFormatterResolver);
        }

        public static async Task<T> DeserializeFileAsync<T>(string path)
        {
            using var stream = FileContentUtility.OpenRead(path);
            if (stream == null)
            {
                return default;
            }

            return await DeserializeAsync<T>(stream).ConfigureAwait(false);
        }

        public static Task SerializeAsync<T>(T @object, Stream stream)
        {
            return JsonSerializer.SerializeAsync(stream, @object, _jsonFormatterResolver);
        }

        public static async Task SerializeFileAsync<T>(T @object, string path)
        {
            using var bufferStream = new MemoryStream();
            await SerializeAsync(@object, bufferStream).ConfigureAwait(false);

            using var outputStream = FileContentUtility.OpenCreate(path);
            bufferStream.Seek(0, SeekOrigin.Begin);

            await bufferStream.CopyToAsync(outputStream).ConfigureAwait(false);
        }
    }
}
