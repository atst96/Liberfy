using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class AccountApi : ApiBase
    {
        internal AccountApi(TwitterApi tokens) : base(tokens) { }

        public Task<UserResponse> VerifyCredentials()
        {
            return this.Api.RestApiGetRequestAsync<UserResponse>("account/verify_credentials");
        }

        public Task<UserResponse> VerifyCredentials(IQuery query)
        {
            return this.Api.RestApiGetRequestAsync<UserResponse>("account/verify_credentials", query);
        }
    }
}
