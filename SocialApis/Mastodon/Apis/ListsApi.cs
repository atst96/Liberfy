using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class ListsApi : TokenApiBase
    {
        internal ListsApi(Tokens tokens) : base(tokens) { }

        public Task<List[]> GetLists()
        {
            return this.Tokens.GetRequestRestApiAsync<List[]>("lists");
        }

        public Task<List[]> GetMembershipLists(long accountId)
        {
            return this.Tokens.GetRequestRestApiAsync<List[]>($"accounts/{ accountId }/lists");
        }

        public Task<Account[]> GetListAccounts(long listId)
        {
            return this.Tokens.GetRequestRestApiAsync<Account[]>($"lists/{ listId }/accounts");
        }

        public Task<List> GetList(long listId)
        {
            return this.Tokens.GetRequestRestApiAsync<List>($"lists/{ listId }");
        }

        public Task<List> Create(string title)
        {
            var query = new Query { ["title"] = title };
            return this.Tokens.PostRequestRestApiAsync<List>("lists", query);
        }

        public Task<List> Update(long listId, string title)
        {
            var query = new Query { ["title"] = title };
            var req = this.Tokens.CreateApiRequest("lists", query, "PUT");
            return this.Tokens.SendRequest<List>(req);
        }

        public Task<List> Delete(long listId)
        {
            var req = this.Tokens.CreateApiRequest($"lists/{ listId }", null, "DELETE");
            return this.Tokens.SendRequest<List>(req);
        }

        public Task AddAccounts(long listId, params long[] accountIds)
        {
            var query = new Query { ["account_ids"] = new UrlArray(accountIds) };

            var req = this.Tokens.CreatePostRequesterApi($"lists/{ listId }/accounts", query);
            return this.Tokens.SendRequestVoid(req);
        }

        public Task RemoveAccounts(long listId, params long[] accountIds)
        {
            var query = new Query { ["account_ids"] = new UrlArray(accountIds) };
            var req = this.Tokens.CreateApiRequest($"lists/{ listId }/accounts", query, "DELETE");
            return this.Tokens.SendRequestVoid(req);
        }
    }
}
