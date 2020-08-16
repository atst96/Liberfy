using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

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

        internal static RateLimit FromHeaders(HttpResponseHeaders headers)
        {
            int limit = GetValueOrDefault(headers, XRateLimitLimit);
            int remaining = GetValueOrDefault(headers, XRateLimitRemaining);
            int resetDate = GetValueOrDefault(headers, XRateLimitReset);

            return new RateLimit
            {
                Limit = limit,
                Remaining = remaining,
                ResetDate = DateTimeOffset.FromUnixTimeSeconds(resetDate),
            };
        }

        private static int GetValueOrDefault(HttpResponseHeaders headers, string key)
        {
            if (headers.TryGetValues(key, out var values))
            {
                if (int.TryParse(values.First(), out int value))
                {
                    return value;
                }
            }

            return default;
        }

        public int Limit { get; private set; }

        public int Remaining { get; private set; }

        public DateTimeOffset ResetDate { get; private set; }
    }
}
