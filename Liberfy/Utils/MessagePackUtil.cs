using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Liberfy.Utilieis;
using MessagePack;

namespace Liberfy.Utils
{
    internal static class MessagePackUtil
    {
        /// <summary>
        /// MessagePackオプション
        /// </summary>
        private static readonly MessagePackSerializerOptions _options = MessagePack.Resolvers.StandardResolverAllowPrivate.Options;

        /// <summary>
        /// ストリームからMessagePackデータをデシリアライズする
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="stream">ストリーム</param>
        /// <returns>Task<typeparamref name="T"/></returns>
        public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
            var @object = await MessagePackSerializer
                .DeserializeAsync<T>(stream, _options)
                .ConfigureAwait(false);

            return @object;
        }

        /// <summary>
        /// ファイルからMessagePackデータをデシリアライズする
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
        /// オブジェクトをMessagePackデータにシリアライズしてStreamに書き込む
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="object">オブジェクト</param>
        /// <param name="stream">出力先ストリーム</param>
        /// <returns>Task</returns>
        public static Task SerializeAsync<T>(T @object, Stream stream)
        {
            return MessagePackSerializer.SerializeAsync(stream, @object, _options);
        }

        /// <summary>
        /// オブジェクトをMessagePackデータにシリアライズしてファイルに書き込む
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
