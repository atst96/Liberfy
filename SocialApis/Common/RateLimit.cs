using System;
using System.Net;

namespace SocialApis
{
    public struct RateLimit
    {
        private const string XRateLimitLimit = "x-rate-limit-limit";
        private const string XRateLimitRemaining = "x-rate-limit-remaining";
        private const string XRateLimitReset = "x-rate-limit-reset";

        internal static RateLimit FromHeaders(WebHeaderCollection header)
        {
            var rateLimit = new RateLimit();

            if (int.TryParse(header[XRateLimitLimit], out int limit))
            {
                rateLimit.Limit = limit;
            }

            if (int.TryParse(header[XRateLimitRemaining], out int remaining))
            {
                rateLimit.Remaining = remaining;
            }

            if (int.TryParse(header[XRateLimitReset], out int unixTime))
            {
                rateLimit.ResetDate = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            }

            return rateLimit;
        }

        public int Limit { get; private set; }

        public int Remaining { get; private set; }

        public DateTimeOffset ResetDate { get; private set; }
    }
}
