using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterApi = SocialApis.Twitter;
using MastodonApi = SocialApis.Mastodon;
using Liberfy.Model;

namespace Liberfy
{
    public static class SocialApisExtensions
    {
        public static TwitterApi.Status GetSourceStatus(this TwitterApi.Status status) => status.RetweetedStatus ?? status;

        public static long GetSourceId(this TwitterApi.Status status) => status.GetSourceStatus().Id;

        public static (string sourceUrl, string sourceName) ParseSource(this TwitterApi.Status status)
        {
            var match = Regexes.TwitterSourceHtml.Match(status.Source);

            return match == null
                ? (string.Empty, status.Source)
                : (match.Groups["url"].Value, match.Groups["name"].Value);
        }
    }
}
