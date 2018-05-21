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
        internal static RateLimit FromHeaders(WebHeaderCollection webHeader)
        {
            var rateLimit = new RateLimit();

            var _limit = webHeader["x-rate-limit-limit"];
            if (int.TryParse(_limit, out var limit))
                rateLimit.Limit = limit;

            var _remaining = webHeader["x-rate-limit-remaining"];
            if (int.TryParse(_remaining, out var remaining))
                rateLimit.Remaining = remaining;

            var _reset = webHeader["x-rate-limit-reset"];
            if (int.TryParse(_reset, out var unixTime))
                rateLimit.ResetDate = DateTimeOffset.FromUnixTimeSeconds(unixTime);

            return rateLimit;
        }

        public int Limit { get; private set; }

        public int Remaining { get; private set; }

        public DateTimeOffset ResetDate { get; private set; }
    }
}
