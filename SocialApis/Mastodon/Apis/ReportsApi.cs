using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class ReportsApi : TokenApiBase
    {
        internal ReportsApi(Tokens tokens) : base(tokens) { }

        public Task<Report[]> GetReports()
        {
            return this.Tokens.GetRequestRestApiAsync<Report[]>("reports");
        }

        public Task<Report> Report(long accountId, long statusId, string comment)
        {
            var query = new Query
            {
                ["account_id"] = accountId,
                ["status_id"]  = statusId,
                ["comment"]    = comment,
            };

            return this.Tokens.PostRequestRestApiAsync<Report>("reports", query);
        }
    }
}
