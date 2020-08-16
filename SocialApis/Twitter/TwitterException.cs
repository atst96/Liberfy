using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Utils;

namespace SocialApis.Twitter
{
    public sealed class TwitterException : Exception
    {
        /// <summary>
        /// レスポンス
        /// </summary>
        public HttpResponseMessage Response { get; }

        internal TwitterException(string message)
            : base(message)
        {
            this.Errors = new TwitterError[0];
        }

        private TwitterException(HttpResponseMessage message, TwitterError[] errors)
            : this(errors?.FirstOrDefault()?.Message)
        {
            this.Response = message;
            this.Errors = errors ?? Array.Empty<TwitterError>();
        }

        private TwitterException(WebException wex, TwitterError[] errors)
            : base(wex.Message, wex)
        {
            this.Errors = errors ?? Array.Empty<TwitterError>();
        }

        public TwitterError[] Errors { get; }

        internal static TwitterException FromWebException(WebException wex)
        {
            using var response = wex.Response.GetResponseStream();

            try
            {
                var errors = JsonUtil.Deserialize<TwitterErrorContainer>(response);

                return new TwitterException(wex, errors.Errors);
            }
            catch (Utf8Json.JsonParsingException ex)
            {
                response.Position = 0;

                using var reader = new StreamReader(response, EncodingUtil.UTF8);
                var message = string.Concat(ex.Message, "\n\n", reader.ReadToEnd());

                throw new Exception(message, ex);
            }
        }

        internal static TwitterException FromWebException(HttpResponseMessage response)
        {
            var content = response.Content;
            var data = content.ReadAsByteArrayAsync().WaitResult();
            using var stream = new MemoryStream(data);

            try
            {
                var errors = JsonUtil.Deserialize<TwitterErrorContainer>(stream);

                return new TwitterException(response, errors.Errors);
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
