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

        internal static IEnumerable<TwitterApi.EntityBase> GetAllEntities(this TwitterApi.Entities entities)
        {
            if (entities == null)
                return Enumerable.Empty<TwitterApi.EntityBase>();

            return new TwitterApi.EntityBase[][]
            {
                entities.Hashtags,
                entities.Symbols,
                entities.Urls,
                entities.UserMentions,
                entities.Media
            }.Merge();
        }

        public static (string sourceUrl, string sourceName) ParseSource(this TwitterApi.Status status)
        {
            var match = Regexes.TwitterSourceHtml.Match(status.Source);

            if (match == null)
                return (string.Empty, status.Source);
            else
                return (match.Groups["url"].Value, match.Groups["name"].Value);
        }

        public static IEnumerable<TwitterApi.EntityBase> EnumerateEntity(this TwitterApi.Entities entities)
        {
            if (entities == null)
                yield break;

            var entitiesList = new TwitterApi.EntityBase[][]
            {
                entities.Hashtags,
                entities.Symbols,
                entities.Urls,
                entities.UserMentions,
                entities.Media
            };

            foreach (var _entities in entitiesList.Where(es => es?.Length > 0))
            {
                foreach (var entity in _entities)
                {
                    yield return entity;
                }
            }
        }

        public static IEnumerable<TwitterApi.EntityBase> SortByStartIndex(this IEnumerable<TwitterApi.EntityBase> collection)
        {
            return collection.OrderBy(e => e.IndexStart);
        }
    }
}
