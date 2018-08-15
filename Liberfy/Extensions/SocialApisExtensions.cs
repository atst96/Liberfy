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

        public static IEnumerable<TwitterApi.EntityBase> ToEntityList(this TwitterApi.Entities entities)
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
                foreach (var entity in _entities)
                    yield return entity;
        }

        public static EntityBase[] ToCommonEntities(this IEnumerable<TwitterApi.EntityBase> allEntities, string plainText)
        {
            var entities = allEntities
                .OrderBy(e => e.IndexStart)
                .GetEnumerator();

            if (!entities.MoveNext())
                return new EntityBase[0];

            var entityList = new LinkedList<EntityBase>();

            var textReader = new SequentialSurrogateTextReader(plainText);

            var entity = entities.Current;

            if (entity.IndexStart != 0)
                textReader.SkipLength(entity.IndexStart);

            while (entity != null)
            {
                var newEntity = default(EntityBase);

                switch (entity)
                {
                    case TwitterApi.HashtagEntity hashtag:
                        newEntity = new HashtagEntity(hashtag);
                        break;

                    case TwitterApi.MediaEntity media:
                        newEntity = new MediaEntity(media);
                        break;

                    case TwitterApi.UrlEntity url:
                        newEntity = new UrlEntity(url);
                        break;

                    case TwitterApi.UserMentionEntity user:
                        newEntity = new MentionEntity(user);
                        break;

                    default:
                        continue;
                }

                newEntity.ActualIndexStart = textReader.Cursor;
                newEntity.ActualLength = textReader.GetNextLength(newEntity.Length);
                entityList.AddLast(newEntity);

                int prevEntityIndexEnd = entity.IndexEnd;

                entity = entities.MoveNext() ? entities.Current : null;

                if (entity == null)
                    break;
                else
                    textReader.SkipLength(entity.IndexStart - prevEntityIndexEnd);
            }

            textReader = null;

            return entityList.ToArray();
        }
    }
}
