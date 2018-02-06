using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SocialApis
{
    public struct RateLimit
    {
        internal void Set(WebHeaderCollection webHeader)
        {
            var _limit = webHeader["x-rate-limit-limit"];
            if (int.TryParse(_limit, out var limit))
                this.Limit = limit;

            var _remaining = webHeader["x-rate-limit-remaining"];
            if (int.TryParse(_remaining, out var remaining))
                this.Remaining = remaining;

            var _reset = webHeader["x-rate-limit-reset"];
            if (int.TryParse(_reset, out var unixTime))
                this.ResetDate = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        }

        public int Limit { get; private set; }

        public int Remaining { get; private set; }

        public DateTimeOffset ResetDate { get; private set; }
    }
}
