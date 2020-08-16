using System.IO;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Utils
{
    /// <summary>
    /// JSONに関するUtilクラス
    /// </summary>
    internal static class JsonUtil
    {
        private static readonly IJsonFormatterResolver _jsonResolver = Utf8Json.Resolvers.StandardResolver.AllowPrivate;

        /// <summary>
        /// <paramref name="value"/>をJSONにシリアライズして<paramref name="stream"/>に書き込む。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">白ライズ結果の書き込み先</param>
        /// <param name="value">シリアライズするオブジェクト</param>
        public static void Serialize<T>(Stream stream, T value)
            => JsonSerializer.Serialize(stream, value, _jsonResolver);

        /// <summary>
        /// <paramref name="value"/>をJSONにシリアライズして<paramref name="stream"/>に書き込む。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">白ライズ結果の書き込み先</param>
        /// <param name="value">シリアライズするオブジェクト</param>
        public static Task SerializeAsync<T>(Stream stream, T value)
            => JsonSerializer.SerializeAsync(stream, value, _jsonResolver);

        /// <summary>
        /// JSON文字列から<typeparamref name="T"/>に変換する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns><typeparamref name="T"/></returns>
        public static T Deserialize<T>(string text)
            => JsonSerializer.Deserialize<T>(text, _jsonResolver);

        /// <summary>
        /// ストリームのJSONを<typeparamref name="T"/>に変換する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns><typeparamref name="T"/></returns>
        public static T Deserialize<T>(Stream stream)
            => JsonSerializer.Deserialize<T>(stream, _jsonResolver);

        /// <summary>
        /// ストリームのJSONを<typeparamref name="T"/>に変換する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns><typeparamref name="T"/></returns>
        public static Task<T> DeserializeAsync<T>(Stream stream)
            => JsonSerializer.DeserializeAsync<T>(stream, _jsonResolver);
    }
}
