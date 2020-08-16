using System;
using System.IO;
using System.Net.Http;
using SocialApis.Utils;

namespace SocialApis.Mastodon
{
    public sealed class MastodonException : Exception
    {
        private MastodonException(MastodonError error)
            : base(error.Error)
        {
            this.Error = error;
        }

        public MastodonError Error { get; }

        internal static MastodonException FromWebException(HttpResponseMessage response)
        {
            var content = response.Content;
            var data = content.ReadAsByteArrayAsync().WaitResult();
            using var stream = new MemoryStream(data);

            try
            {
                var erorr = JsonUtil.Deserialize<MastodonError>(stream);

                return new MastodonException(erorr);
            }
            catch (Utf8Json.JsonParsingException ex)
            {
                stream.Position = 0;

                using var reader = new StreamReader(stream, EncodingUtil.UTF8);
                var message = string.Concat(ex.Message, "\n\n", reader.ReadToEnd());

                throw new Exception(message, ex);
            }
        }
    }
}
