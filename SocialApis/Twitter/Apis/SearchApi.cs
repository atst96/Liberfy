using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public class SearchApi : TokenApiBase
    {
        internal SearchApi(Tokens tokens) : base(tokens) { }

        public Task<ListedResponse<Status>> Search(string text)
        {
            return this.Search(new Query { ["q"] = text });
        }

        public Task<ListedResponse<Status>> Search(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Status>>("search/tweets", query);
        }
    }
}
