using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy
{
    internal static class FileContentUtility
    {
        private static IFormatterResolver _msgPackResolver { get; } = MessagePack.Resolvers.StandardResolverAllowPrivate.Instance;
        private static IJsonFormatterResolver _utf8jsonResolver { get; } = Utf8Json.Resolvers.StandardResolver.AllowPrivate;

        public static bool TryOpenReadFile(string path, out FileStream stream)
        {
            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.Read);
                return true;
            }
            else
            {
                stream = null;
                return false;
            }
        }

        public static FileStream OpenWriteFile(string path)
        {
            return File.Open(path, FileMode.Create, FileAccess.Write);
        }

        public static T DeserializeJson<T>(Stream stream) where T : class
        {
            return JsonSerializer.Deserialize<T>(stream, _utf8jsonResolver);
        }

        public static Task<T> DeserializeJsonAsync<T>(Stream stream) where T : class
        {
            return JsonSerializer.DeserializeAsync<T>(stream, _utf8jsonResolver);
        }

        public static T DeserializeJsonFile<T>(string path) where T : class
        {
            if (TryOpenReadFile(path, out var stream))
            {
                using (stream)
                {
                    return DeserializeJson<T>(stream);
                }
            }

            return default;
        }

        public static async Task<T> DeserializeJsonFileAsync<T>(string path) where T : class
        {
            if (TryOpenReadFile(path, out var stream))
            {
                using (stream)
                {
                    return await DeserializeJsonAsync<T>(stream).ConfigureAwait(false);
                }
            }

            return default;
        }

        public static T DeserializeMsgPack<T>(Stream stream) where T : class
        {
            return MessagePackSerializer.Deserialize<T>(stream, _msgPackResolver);
        }

        public static T DeserializeMsgPackFromFile<T>(string path) where T : class
        {
            if (TryOpenReadFile(path, out var stream))
            {
                using (stream)
                {
                    return DeserializeMsgPack<T>(stream);
                }
            }

            return default;
        }

        public static byte[] SerializeJson<T>(T obj) where T : class
        {
            return JsonSerializer.Serialize(obj, _utf8jsonResolver);
        }

        public static void SerializeJson<T>(T obj, Stream stream) where T : class
        {
            JsonSerializer.Serialize(stream, obj, _utf8jsonResolver);
        }

        public static void SerializeJsonToFile<T>(T obj, string path) where T : class
        {
            byte[] data = SerializeJson(obj);

            using (var stream = OpenWriteFile(path))
            {
                stream.Write(data, 0, data.Length);
            }
        }

        public static byte[] SerializeMsgPack<T>(T obj) where T : class
        {
            return MessagePackSerializer.Serialize(obj, _msgPackResolver);
        }

        public static void SerializeMsgPack<T>(T obj, Stream stream) where T : class
        {
            MessagePackSerializer.Serialize(stream, obj, _msgPackResolver);
        }

        public static void SerializeMsgPackToFile<T>(T obj, string path) where T : class
        {
            byte[] data = SerializeMsgPack(obj);

            if (data.Length > 0)
            {
                using (var stream = OpenWriteFile(path))
                {
                    stream.Write(data, 0, data.Length);
                }
            }
        }
    }
}
