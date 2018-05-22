using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class AccountApi : TokenApiBase
    {
        internal AccountApi(Tokens tokens) : base(tokens) { }

        public Task<UserResponse> VerifyCredentials()
        {
            return this.Tokens.GetRequestRestApiAsync<UserResponse>("account/verify_credentials");
        }

        public Task<UserResponse> VerifyCredentials(IQuery query)
        {
            return this.Tokens.GetRequestRestApiAsync<UserResponse>("account/verify_credentials", query);
        }
    }
}
