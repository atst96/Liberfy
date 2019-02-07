using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class CollectionsApi : ApiBase
    {
        internal CollectionsApi(TwitterApi tokens) : base(tokens) { }
    }
}
