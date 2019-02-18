using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class ListsApi : ApiBase
    {
        internal ListsApi(MastodonApi tokens) : base(tokens) { }

        public Task<List[]> GetLists()
        {
            return this.Api.RestApiGetRequestAsync<List[]>("lists");
        }

        public Task<List[]> GetMembershipLists(long accountId)
        {
            return this.Api.RestApiGetRequestAsync<List[]>($"accounts/{ accountId }/lists");
        }

        public Task<Account[]> GetListAccounts(long listId)
        {
            return this.Api.RestApiGetRequestAsync<Account[]>($"lists/{ listId }/accounts");
        }

        public Task<List> GetList(long listId)
        {
            return this.Api.RestApiGetRequestAsync<List>($"lists/{ listId }");
        }

        public Task<List> Create(string title)
        {
            var query = new Query { ["title"] = title };
            return this.Api.RestApiPostRequestAsync<List>("lists", query);
        }

        public Task<List> Update(long listId, string title)
        {
            var query = new Query { ["title"] = title };
            return this.Api.RestApiPutRequestAsync<List>($"lists/{ listId }", query);
        }

        public Task<List> Delete(long listId)
        {
            return this.Api.RestApiDeleteRequestAsync<List>($"lists/{ listId }");
        }

        public Task AddAccounts(long listId, params long[] accountIds)
        {
            var query = new Query { ["account_ids"] = new QueryArrayItem(accountIds) };

            return this.Api.RestApiPostRequestAsync($"lists/{ listId }/accounts");
        }

        public Task RemoveAccounts(long listId, params long[] accountIds)
        {
            var query = new Query { ["account_ids"] = new QueryArrayItem(accountIds) };

            return this.Api.RestApiDeleteRequestAsync($"lists/{ listId }/accounts", query);
        }
    }
}
