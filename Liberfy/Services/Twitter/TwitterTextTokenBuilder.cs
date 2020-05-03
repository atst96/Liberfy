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
    /// <summary>
    /// ツイート表示用エンティティを生成するクラス
    /// </summary>
    internal class TwitterTextTokenBuilder : ITextEntityBuilder
    {
        /// <summary>
        /// 表示内容
        /// </summary>
        private string _content;

        /// <summary>
        /// 変換前エンティティのリスト
        /// </summary>
        private TwitterApi.Entities _entityGroup;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content">文字列</param>
        /// <param name="entityGroup">エンティティ</param>
        public TwitterTextTokenBuilder(string content, TwitterApi.Entities entityGroup)
        {
            this._content = content ?? throw new ArgumentNullException(nameof(content));
            this._entityGroup = entityGroup;
        }

        /// <summary>
        /// エンティティを列挙する
        /// </summary>
        /// <param name="entityGroup"></param>
        /// <returns>エンティティ一覧</returns>
        public static TwitterApi.EntityBase[] EnumerateEntity(TwitterApi.Entities entityGroup)
        {
            return new TwitterApi.EntityBase[][]
            {
                entityGroup?.Hashtags,
                entityGroup?.Symbols,
                entityGroup?.Urls,
                entityGroup?.UserMentions,
                entityGroup?.Media,
            }
            .Where(entities => entities?.Length > 0)
            .SelectMany(entities => entities)
            .OrderBy(entity => entity.IndexStart)
            .ToArray();
        }

        /// <summary>
        /// エンティティを変換する
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="rawEntity"></param>
        /// <returns></returns>
        private static IEntity CreateEntity(SequentialSurrogateTextReader textReader, TwitterApi.EntityBase rawEntity)
        {
            var content = textReader.Content;
            int startIndex = textReader.RawCursor;
            int length = textReader.GetRawLength(rawEntity.IndexEnd - rawEntity.IndexStart);

            return rawEntity switch
            {
                TwitterApi.HashtagEntity hashtag => new HashtagEntity(content.Substring(startIndex, length), hashtag),
                TwitterApi.MediaEntity media => new MediaEntity(media),
                TwitterApi.UrlEntity url => new UrlEntity(url),
                TwitterApi.UserMentionEntity user => new MentionEntity(content.Substring(startIndex, length), user),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// 表示用のエンティティを生成する
        /// </summary>
        /// <returns>エンティティのリスト</returns>
        public IReadOnlyList<IEntity> Build()
        {
            var content = this._content;
            var rawEntities = EnumerateEntity(this._entityGroup);

            if (rawEntities.Length == 0)
            {
                // 処理対象のエンティティがなければ早期に抜ける
                return string.IsNullOrEmpty(content)
                    ? Array.Empty<IEntity>()
                    : new[] { new PlainTextEntity(content.DecodeHtml()) };
            }

            var textReader = new SequentialSurrogateTextReader(content);
            var entities = new List<IEntity>((rawEntities.Length * 2) + 1);

            int entityIdx = 0;
            var rawEntity = rawEntities[entityIdx++];

            
            // 最初のエンティティに関する処理
            {
                int firstEntityTextStartIndex = rawEntities[0].IndexStart;

                if (firstEntityTextStartIndex != 0)
                {
                    // 最初のエンティティが先頭始まりでない（エンティティの前に文字列が存在する）場合、
                    // 　エンティティより前の文字列を追加する
                    var entityText = textReader.ReadLength(firstEntityTextStartIndex);

                    entities.Add(new PlainTextEntity(entityText.DecodeHtml()));
                }
            }

            // 2番目以降のエンティティに関する処理
            do
            {
                // エンティティを変換して追加
                var entity = CreateEntity(textReader, rawEntity);
                entities.Add(entity);

                int previousEntityTextEndIndex = rawEntity.IndexEnd;

                // 次のエンティティまでの文字列を追加
                if (entityIdx < rawEntities.Length)
                {
                    // 次のエンティティが存在する場合は、現エンティティの終了から次エンティティの開始までの文字列を追加する
                    rawEntity = rawEntities[entityIdx++];

                    var entityText = textReader.ReadLength(rawEntity.IndexStart - previousEntityTextEndIndex);

                    entities.Add(new PlainTextEntity(entityText.DecodeHtml()));
                }
                else
                {
                    rawEntity = null;

                    if (previousEntityTextEndIndex < content.Length)
                    {
                        var entityText = textReader.ReadToEnd();

                        entities.Add(new PlainTextEntity(entityText.DecodeHtml()));
                    }

                    break;
                }
            }
            while (rawEntity != null);

            var result = entities.ToArray();

            entities.Clear();

            return result;
        }
    }
}
