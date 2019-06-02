using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Model;
using TwitterApi = SocialApis.Twitter;

namespace Liberfy.Services.Twitter
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

        private static void InsertEntityImpl(IList<TwitterApi.EntityBase[]> list, TwitterApi.EntityBase[] entities, ref int count)
        {
            if (entities?.Length > 0)
            {
                list.Add(entities);
                count += entities.Length;
            }
        }

        public static TwitterApi.EntityBase[] EnumerateEntity(TwitterApi.Entities entities)
        {
            if (entities == null)
            {
                return new TwitterApi.EntityBase[0];
            }

            var entitiesList = new List<TwitterApi.EntityBase[]>(5);

            int count = 0;

            InsertEntityImpl(entitiesList, entities.Hashtags, ref count);
            InsertEntityImpl(entitiesList, entities.Symbols, ref count);
            InsertEntityImpl(entitiesList, entities.Urls, ref count);
            InsertEntityImpl(entitiesList, entities.UserMentions, ref count);
            InsertEntityImpl(entitiesList, entities.Media, ref count);

            if (count == 0)
            {
                return new TwitterApi.EntityBase[0];
            }

            var entityList = new TwitterApi.EntityBase[count];
            int idx = 0;

            foreach (var entities_ in entitiesList)
            {
                foreach (var entity in entities_)
                {
                    entityList[idx++] = entity;
                }
            }

            Array.Sort(entityList, TextEntityStartPositionCompare.Instance);

            entitiesList.Clear();
            entitiesList = null;

            return entityList;
        }

        private static IEntity CreateElement(SequentialSurrogateTextReader textReader, TwitterApi.EntityBase entity)
        {
            IEntity element = default;

            var sourceText = textReader.String;
            int indexStart = textReader.Cursor;
            int length = textReader.GetNextLength(entity.IndexEnd - entity.IndexStart);

            switch (entity)
            {
                case TwitterApi.HashtagEntity hashtag:
                    element = new HashtagEntity(sourceText.Substring(indexStart, length), hashtag);
                    break;

                case TwitterApi.MediaEntity media:
                    element = new MediaEntity(media);
                    break;

                case TwitterApi.UrlEntity url:
                    element = new UrlEntity(url);
                    break;

                case TwitterApi.UserMentionEntity user:
                    element = new MentionEntity(sourceText.Substring(indexStart, length), user);
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (element.DisplayText == null)
            {
                element.DisplayText = sourceText.Substring(indexStart, length);
            }

            return element;
        }

        public IReadOnlyList<IEntity> Build()
        {
            var sourceText = this._text;

            var sourceTextEntities = EnumerateEntity(this._entities);

            if (sourceTextEntities.Length == 0)
            {
                if (string.IsNullOrEmpty(sourceText))
                {
                    return new IEntity[0];
                }
                else
                {
                    return new IEntity[]
                    {
                        new PlainTextEntity(sourceText.DecodeHtml())
                    };
                }
            }

            var textElementList = new List<IEntity>((sourceTextEntities.Length * 2) + 1);
            var textReader = new SequentialSurrogateTextReader(sourceText);

            int entityIdx = 0;
            var entity = sourceTextEntities[entityIdx++];

            int firstEntitySourceTextPosition = sourceTextEntities[0].IndexStart;

            if (firstEntitySourceTextPosition != 0)
            {
                var entityText = textReader.ReadLength(firstEntitySourceTextPosition);

                textElementList.Add(new PlainTextEntity(entityText.DecodeHtml()));
            }

            do
            {
                var textElement = CreateElement(textReader, entity);
                textElementList.Add(textElement);

                int previousSourceTextPosition = entity.IndexEnd;

                if (entityIdx < sourceTextEntities.Length)
                {
                    // 次のエンティティが在る場合は現在のエンティティの終わりから次の開始位置までの文字列を追加する

                    entity = sourceTextEntities[entityIdx++];

                    var entityText = textReader.ReadLength(entity.IndexStart - previousSourceTextPosition);

                    textElementList.Add(new PlainTextEntity(entityText.DecodeHtml()));
                }
                else
                {
                    entity = null;

                    if (previousSourceTextPosition < sourceText.Length)
                    {
                        var entityText = textReader.ReadToEnd();

                        textElementList.Add(new PlainTextEntity(entityText.DecodeHtml()));
                    }

                    break;
                }
            }
            while (entity != null);

            var result = textElementList.ToArray();

            textElementList.Clear();

            return result;
        }

        ~TwitterTextTokenBuilder()
        {
            this._text = null;
            this._entities = null;
        }
    }
}
