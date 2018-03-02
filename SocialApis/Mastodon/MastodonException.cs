using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
#pragma warning disable RCS1194 // Implement exception constructors.
    public class MastodonException : Exception
#pragma warning restore RCS1194 // Implement exception constructors.
    {
        internal MastodonException(MastodonError error, WebException exception)
            : base(error.ErrorDescription, exception)
        {
            this.Error = error;
        }

        public MastodonError Error { get; }
    }
}
