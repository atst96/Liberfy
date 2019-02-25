using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis
{
    internal static class JsonUtility
    {
        private static readonly IJsonFormatterResolver _jsonResolver;

        static JsonUtility()
        {
            _jsonResolver = Utf8Json.Resolvers.StandardResolver.AllowPrivate;
        }

        public static void Serialize<T>(Stream stream, T value)
        {
            JsonSerializer.Serialize(stream, value, _jsonResolver);
        }

        public static Task SerializeAsync<T>(Stream stream, T value)
        {
            return JsonSerializer.SerializeAsync(stream, value, _jsonResolver);
        }

        public static T Deserialize<T>(string stream)
        {
            return JsonSerializer.Deserialize<T>(stream, _jsonResolver);
        }

        public static T Deserialize<T>(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream, _jsonResolver);
        }

        public static Task<T> DeserializeAsync<T>(Stream stream)
        {
            return JsonSerializer.DeserializeAsync<T>(stream, _jsonResolver);
        }
    }
}
