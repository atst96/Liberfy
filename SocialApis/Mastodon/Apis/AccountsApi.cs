using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class AccountsApi : TokenApiBase
    {
        internal AccountsApi(Tokens tokens) : base(tokens) { }

        public Task<Account> VerifyCredentials()
        {
            return this.Tokens.GetRequestRestApiAsync<Account>("accounts/verify_credentials");
        }
    }
}
