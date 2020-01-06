using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Liberfy.Model;
using Sgml;

namespace Liberfy.Services.Mastodon
{
    /// <summary>
    /// トゥート表示用のエンティティを生成するクラス
    /// </summary>
    internal class MastodonTextEntityBuilder : ITextEntityBuilder
    {
        /// <summary>
        /// 表示内容
        /// </summary>
        private readonly string _content;

        /// <summary>
        /// 絵文字一覧
        /// </summary>
        private readonly SocialApis.Mastodon.Emoji[] _emojis;

        /// <summary>
        /// 絵文字検出用の正規表現
        /// </summary>
        private readonly static Regex EmojiRegex = new Regex(":(?<code>[a-zA-Z0-9_\\-]+):", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// メンションのクラス名
        /// </summary>
        private static readonly string[] MentionElementClassNames = { "u-url", "mention" };

        /// <summary>
        /// メンションの親クラス名
        /// </summary>
        private const string MentionParentElementClassName = "h-card";

        /// <summary>
        /// ハッシュタグのクラス名
        /// </summary>
        private static readonly string[] HashtagElementClassNames = { "mention", "hashtag" };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content"></param>
        /// <param name="emojis"></param>
        public MastodonTextEntityBuilder(string content, SocialApis.Mastodon.Emoji[] emojis)
        {
            this._content = content;
            this._emojis = emojis;
        }

        /// <summary>
        /// エンティティのコレクションに文字列と絵文字を追加する
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="text"></param>
        /// <param name="emojis"></param>
        private static void AddText(TextEntityCollection entities, string text, SocialApis.Mastodon.Emoji[] emojis)
        {
            if (emojis?.Length == 0)
            {
                entities.AddText(text);
                return;
            }

            var matches = EmojiRegex.Matches(text);
            if (matches.Count == 0)
            {
                entities.AddText(text);
            }

            if (matches[0].Index != 0)
            {
                var firstText = text.Substring(0, matches[0].Index);

                entities.AddText(firstText);
            }

            for (int i = 0; i < matches.Count; ++i)
            {
                var m = matches[i];

                var shortCode = m.Groups["code"].Value;

                SocialApis.Mastodon.Emoji emoji = default;
                foreach (var e in emojis)
                {
                    if (e.ShortCode == shortCode)
                    {
                        emoji = e;
                        break;
                    }
                }

                if (emoji == default)
                {
                    entities.AddText(m.Value);
                }
                else
                {
                    entities.Add(new EmojiEntity(emoji));
                }

                if (i < matches.Count - 1)
                {
                    var nextMatch = matches[i + 1];
                    var content = text.Substring(m.Index + m.Value.Length, nextMatch.Index - m.Index - m.Value.Length);

                    entities.AddText(content);
                }
                else if (m.Index + m.Value.Length != text.Length)
                {
                    var content = text.Substring(m.Index + m.Value.Length);

                    entities.AddText(content);
                }
            }
        }

        /// <summary>
        /// XElementから文字列を抽出する
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private string GetString(XElement element)
        {
            var text = "";

            foreach (var node in element.Nodes())
            {
                if (node is XText textNode)
                {
                    text += textNode.Value;
                    continue;
                }

                if (node is XElement elementNode)
                {
                    var classes = (elementNode.Attribute("class")?.Value ?? "").Split(" ");

                    if (classes.Contains("invisible"))
                    {
                        continue;
                    }

                    text += this.GetString(elementNode);
                    continue;
                }
            }

            return text;
        }

        /// <summary>
        /// XElementからエンティティを生成する
        /// </summary>
        /// <param name="entityCollection"></param>
        /// <param name="rootEntity"></param>
        private void ParseEntity(TextEntityCollection entityCollection, XElement rootEntity)
        {
            foreach (var node in rootEntity.Nodes())
            {
                if (node is XText textNode)
                {
                    // 文字列
                    AddText(entityCollection, textNode.Value, this._emojis);
                    continue;
                }

                if (node is XElement elementNode)
                {
                    var tagName = elementNode.Name;

                    if (tagName == "br")
                    {
                        // 改行
                        entityCollection.AddNewLine();
                        continue;
                    }

                    if (tagName == "a")
                    {
                        // アンカーリンク
                        var href = elementNode.Attribute("href")?.Value;
                        var text = this.GetString(elementNode);
                        var classes = elementNode.GetClassNames();

                        if (elementNode.Parent is XElement parentElement)
                        {
                            if (MentionElementClassNames.All(classes.Contains))
                            {
                                var parentNodeClasses = parentElement.GetClassNames();
                                if (parentNodeClasses.Contains(MentionParentElementClassName))
                                {
                                    // メンション
                                    var anchorText = this.GetString(elementNode);
                                    entityCollection.Add(new MentionEntity(anchorText, anchorText.Substring(1)));
                                    continue;
                                }
                            }
                        }
                        else if (HashtagElementClassNames.All(classes.Contains))
                        {
                            // ハッシュタグ
                            var hashtagText = this.GetString(elementNode);
                            entityCollection.Add(new HashtagEntity(hashtagText));
                            continue;
                        }

                        entityCollection.Add(new UrlEntity(href, text));
                        continue;
                    }

                    if (tagName == "span")
                    {
                        // spanタグ
                        this.ParseEntity(entityCollection, elementNode);
                        continue;
                    }

                    if (tagName == "p")
                    {
                        // pタグ

                        // 直前の要素があれば改行する
                        if (entityCollection.LastEntity != null)
                        {
                            entityCollection.AddNewLine();
                        }

                        this.ParseEntity(entityCollection, elementNode);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 表示用のエンティティを生成する
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IEntity> Build()
        {
            var content = this._content.Replace("<br>", "<br/>", StringComparison.OrdinalIgnoreCase);

            var entities = new TextEntityCollection();

            using var sgmlReader = new SgmlReader
            {
                DocType = "html",
                IgnoreDtd = true,
                CaseFolding = CaseFolding.ToLower,
                InputStream = new StringReader(string.Concat("<div>", content, "</div>")),
            };

            var rootElement = XElement.Load(sgmlReader, LoadOptions.PreserveWhitespace);

            this.ParseEntity(entities, rootElement);

            return entities.ToArray();
        }
    }
}
