using System.IO;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy.Utilieis
{
    /// <summary>
    /// JSON処理に関するクラス
    /// </summary>
    internal static class JsonUtil
    {
        /// <summary>
        /// JSONフォーマッタ
        /// </summary>
        private static IJsonFormatterResolver _jsonFormatterResolver { get; } = Utf8Json.Resolvers.StandardResolver.AllowPrivate;

        /// <summary>
        /// ストリームからJSONデータをデシリアライズする
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="stream">ストリーム</param>
        /// <returns>Task<typeparamref name="T"/></returns>
        public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
            var @object = await JsonSerializer
                .DeserializeAsync<T>(stream, _jsonFormatterResolver)
                .ConfigureAwait(false);

            if (@object is IJsonFile jsonObject)
            {
                jsonObject.OnDeserialized();
            }

            return @object;
        }

        /// <summary>
        /// ファイルからJSONデータをデシリアライズする
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="path">ファイルパス</param>
        /// <returns>Task<typeparamref name="T"/></returns>
        public static async Task<T> DeserializeFileAsync<T>(string path)
        {
            using var stream = FileContentUtil.OpenRead(path, isAsync: true);

            if (stream == null)
            {
                return default;
            }

            return await DeserializeAsync<T>(stream).ConfigureAwait(false);
        }

        /// <summary>
        /// オブジェクトをJSONデータにシリアライズしてStreamに書き込む
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="object">オブジェクト</param>
        /// <param name="stream">出力先ストリーム</param>
        /// <returns>Task</returns>
        public static Task SerializeAsync<T>(T @object, Stream stream)
        {
            if (@object is IJsonFile jsonObject)
            {
                jsonObject.OnSerialize();
            }

            return JsonSerializer.SerializeAsync(stream, @object, _jsonFormatterResolver);
        }

        /// <summary>
        /// オブジェクトをJSONデータにシリアライズしてファイルに書き込む
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="object">オブジェクト</param>
        /// <param name="path">ファイルパス</param>
        /// <returns>Task</returns>
        public static async Task SerializeFileAsync<T>(T @object, string path)
        {
            using var buffer = new MemoryStream();
            await SerializeAsync(@object, buffer).ConfigureAwait(false);

            using var output = FileContentUtil.OpenCreate(path, isAsync: true, bufferSize: (int)buffer.Length);
            buffer.Seek(0, SeekOrigin.Begin);

            await buffer.CopyToAsync(output).ConfigureAwait(false);
        }
    }
}
