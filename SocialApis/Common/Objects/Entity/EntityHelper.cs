using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Common
{
    public static class EntityHelper
    {
        public static IEnumerable<Twitter.EntityBase> ToEntityList(this Twitter.Entities entities)
        {
            if (entities == null)
                yield break;

            var entitiesList = new Twitter.EntityBase[][]
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

        public static EntityBase[] ToCommonEntities(this IEnumerable<Twitter.EntityBase> allEntities, string plainText)
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
                    case Twitter.HashtagEntity hashtag:
                        newEntity = new HashtagEntity(hashtag);
                        break;

                    case Twitter.MediaEntity media:
                        newEntity = new MediaEntity(media);
                        break;

                    case Twitter.UrlEntity url:
                        newEntity = new UrlEntity(url);
                        break;

                    case Twitter.UserMentionEntity user:
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
