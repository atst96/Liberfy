using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public sealed class MastodonException : Exception
    {
        private MastodonException(MastodonError error, WebException exception)
            : base(error.Error, exception)
        {
            this.Error = error;
        }

        public MastodonError Error { get; }

        public static MastodonException FromWebException(WebException wex)
        {
            using var response = wex.Response.GetResponseStream();

            try
            {
                var errors = JsonUtility.Deserialize<MastodonError>(response);

                return new MastodonException(errors, wex);
            }
            catch (Utf8Json.JsonParsingException ex)
            {
                response.Position = 0;

                var message = ex.Message;
                using var reader = new StreamReader(response, EncodingUtility.UTF8);

                message += "\n\n" + reader.ReadToEnd();

                throw new Exception(message, ex);
            }
        }
    }
}
