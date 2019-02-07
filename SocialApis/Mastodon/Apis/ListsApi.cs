using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class ListsApi : ApiBase
    {
        internal ListsApi(MastodonApi tokens) : base(tokens) { }

        public Task<List[]> GetLists()
        {
            return this.Api.GetRequestRestApiAsync<List[]>("lists");
        }

        public Task<List[]> GetMembershipLists(long accountId)
        {
            return this.Api.GetRequestRestApiAsync<List[]>($"accounts/{ accountId }/lists");
        }

        public Task<Account[]> GetListAccounts(long listId)
        {
            return this.Api.GetRequestRestApiAsync<Account[]>($"lists/{ listId }/accounts");
        }

        public Task<List> GetList(long listId)
        {
            return this.Api.GetRequestRestApiAsync<List>($"lists/{ listId }");
        }

        public Task<List> Create(string title)
        {
            var query = new Query { ["title"] = title };
            return this.Api.PostRequestRestApiAsync<List>("lists", query);
        }

        public Task<List> Update(long listId, string title)
        {
            var query = new Query { ["title"] = title };
            var req = this.Api.CreateApiRequest("lists", query, "PUT");
            return this.Api.SendRequest<List>(req);
        }

        public Task<List> Delete(long listId)
        {
            var req = this.Api.CreateApiRequest($"lists/{ listId }", null, "DELETE");
            return this.Api.SendRequest<List>(req);
        }

        public Task AddAccounts(long listId, params long[] accountIds)
        {
            var query = new Query { ["account_ids"] = new UrlArray(accountIds) };

            var req = this.Api.CreatePostRequesterApi($"lists/{ listId }/accounts", query);
            return this.Api.SendRequestVoid(req);
        }

        public Task RemoveAccounts(long listId, params long[] accountIds)
        {
            var query = new Query { ["account_ids"] = new UrlArray(accountIds) };
            var req = this.Api.CreateApiRequest($"lists/{ listId }/accounts", query, "DELETE");
            return this.Api.SendRequestVoid(req);
        }
    }
}
