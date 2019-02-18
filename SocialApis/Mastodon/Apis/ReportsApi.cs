using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class ReportsApi : ApiBase
    {
        internal ReportsApi(MastodonApi tokens) : base(tokens) { }

        public Task<Report[]> GetReports()
        {
            return this.Api.RestApiGetRequestAsync<Report[]>("reports");
        }

        public Task<Report> Report(long accountId, long statusId, string comment)
        {
            var query = new Query
            {
                ["account_id"] = accountId,
                ["status_id"]  = statusId,
                ["comment"]    = comment,
            };

            return this.Api.RestApiPostRequestAsync<Report>("reports", query);
        }
    }
}
