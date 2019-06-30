using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Utilieis
{
    internal static class MessagePackUtility
    {
        private static IFormatterResolver _formatterResolver { get; } = MessagePack.Resolvers.StandardResolverAllowPrivate.Instance;

        public static Task<T> DeserializeAsync<T>(Stream stream)
        {
            return MessagePackSerializer.DeserializeAsync<T>(stream, _formatterResolver);
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
            return MessagePackSerializer.SerializeAsync(stream, @object, _formatterResolver);
        }

        public static async Task SerializeFielAsync<T>(T @object, string path)
        {
            using var bufferStream = new MemoryStream();
            await SerializeAsync(@object, bufferStream).ConfigureAwait(false);

            using var outputStream = FileContentUtility.OpenCreate(path);
            bufferStream.Seek(0, SeekOrigin.Begin);

            await bufferStream.CopyToAsync(outputStream).ConfigureAwait(false);
        }
    }
}
