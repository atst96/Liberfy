using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public sealed class MastodonException : Exception
    {
        private MastodonException(MastodonError error, WebException exception)
            : base(error.ErrorDescription, exception)
        {
            this.Error = error;
        }

        public MastodonError Error { get; }

        public static MastodonException FromWebException(WebException wex)
        {
            using (var response = wex.Response.GetResponseStream())
            {
                var errors = JsonUtility.Deserialize<MastodonError>(response);

                return new MastodonException(errors, wex);
            }
        }
    }
}
