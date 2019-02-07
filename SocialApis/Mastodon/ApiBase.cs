using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public abstract class ApiBase
    {
        protected MastodonApi Api { get; }

        protected ApiBase(MastodonApi tokens)
        {
            this.Api = tokens;
        }
    }
}
