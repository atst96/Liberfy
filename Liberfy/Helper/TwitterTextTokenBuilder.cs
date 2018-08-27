using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Model;
using TwitterApi = SocialApis.Twitter;

namespace Liberfy
{
    internal class TwitterTextTokenBuilder : ITextEntityBuilder
    {
        private string _text;
        private TwitterApi.Entities _entities;

        public TwitterTextTokenBuilder(string text, TwitterApi.Entities entities)
        {
            this._text = text ?? throw new ArgumentNullException(nameof(text));
            this._entities = entities;
        }

        public IEnumerable<EntityBase> Build()
        {
            var text = this._text;

            

            var entities = _entities?
                .EnumerateEntity()
                .SortByStartIndex()
                .GetEnumerator();

            if (entities == null || !entities.MoveNext())
            {
                return string.IsNullOrEmpty(text)
                    ? new EntityBase[0]
                    : new EntityBase[1] { new PlainTextEntity(text, 0, text.Length) };
            }

            var entityList = new LinkedList<EntityBase>();
            var textReader = new SequentialSurrogateTextReader(text);

            var entity = entities.Current;

            if (entity.IndexStart != 0)
            {
                entityList.AddLast(new PlainTextEntity(textReader.ReadLength(entity.IndexStart), 0, entity.IndexStart - 1));
            }

            while (entity != null)
            {
                var newEntity = default(EntityBase);

                int indexStart = textReader.Cursor;
                int length = textReader.GetNextLength(entity.IndexEnd - entity.IndexStart);

                switch (entity)
                {
                    case TwitterApi.HashtagEntity hashtag:
                        newEntity = new HashtagEntity(text.Substring(indexStart, length), hashtag);
                        break;

                    case TwitterApi.MediaEntity media:
                        newEntity = new MediaEntity(media);
                        break;

                    case TwitterApi.UrlEntity url:
                        newEntity = new UrlEntity(url);
                        break;

                    case TwitterApi.UserMentionEntity user:
                        newEntity = new MentionEntity(text.Substring(indexStart, length), user);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                newEntity.ActualIndexStart = indexStart;
                newEntity.ActualLength = length;

                if (newEntity.DisplayText == null)
                {
                    newEntity.DisplayText = text.Substring(newEntity.ActualIndexStart, newEntity.ActualLength);
                }

                entityList.AddLast(newEntity);

                int prevEntityIndexEnd = entity.IndexEnd;

                entity = entities.MoveNext() ? entities.Current : null;

                if (entity == null)
                {
                    if (prevEntityIndexEnd < text.Length)
                    {
                        entityList.AddLast(new PlainTextEntity(textReader.ReadToEnd(), prevEntityIndexEnd, text.Length - prevEntityIndexEnd));
                    }

                    break;
                }
                else
                {
                    entityList.AddLast(new PlainTextEntity(textReader.ReadLength(entity.IndexStart - prevEntityIndexEnd), prevEntityIndexEnd, entity.IndexStart - prevEntityIndexEnd));
                }
            }

            textReader = null;

            return entityList.ToArray();
        }

        ~TwitterTextTokenBuilder()
        {
            this._text = null;
            this._entities = null;
        }
    }
}
