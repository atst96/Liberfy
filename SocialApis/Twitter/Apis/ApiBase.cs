using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public abstract class ApiBase
    {
        protected TwitterApi Api { get; }

        internal ApiBase(TwitterApi api)
        {
            this.Api = api ?? throw new ArgumentNullException(nameof(api));
        }
    }
}
