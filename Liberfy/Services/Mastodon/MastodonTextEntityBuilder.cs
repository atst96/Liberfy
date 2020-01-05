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
        private readonly static Regex _emojiRegex = new Regex(":(?<code>[a-zA-Z0-9_\\-]+):", RegexOptions.Multiline | RegexOptions.Compiled);

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
        /// エンティティのコレクションに文字列を追加する
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

            var matches = _emojiRegex.Matches(text);
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
        /// 表示用のエンティティを生成する
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IEntity> Build()
        {
            const string externalLinkRel = "nofollow noopener";

            const string hashtagElementClassName = "mention hashtag";

            const string mentionContainerClassName = "h-card";
            const string mentionAnchorClassName = "u-url mention";

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

            foreach (var topParagraphElement in rootElement.Elements())
            {
                if (topParagraphElement.Name != "p")
                    throw new NotSupportedException();

                if (entities.Count > 0)
                {
                    AddText(entities, "\n", this._emojis);
                }

                foreach (var secLvNode in topParagraphElement.Nodes())
                {
                    if (secLvNode is XText secLvTextNode)
                    {
                        var value = secLvTextNode.Value;
                        AddText(entities, value, this._emojis);
                    }
                    else if(secLvNode is XElement secLvEl)
                    {
                        if (secLvEl.Name == "br")
                        {
                            entities.AddText("\n");
                        }
                        else if (secLvEl.Name == "a")
                        {
                            var classNames = secLvEl.Attribute("class")?.Value;
                            var rels = secLvEl.Attribute("rel")?.Value;

                            if (classNames == hashtagElementClassName)
                            {
                                var text = secLvEl.Value;
                                // Hashtag
                                entities.Add(new HashtagEntity(text));
                            }
                            else if (rels == externalLinkRel)
                            {
                                var link = secLvEl.Attribute("href")?.Value;

                                var text = default(string);

                                if (secLvEl.HasElements)
                                {
                                    var texts = secLvEl.Elements()
                                        .Where(thirdEl => thirdEl.Attribute("class")?.Value != "invisible")
                                        .Select(thirdEl => thirdEl.Value);

                                    text = string.Concat(texts);
                                }
                                else
                                {
                                    text = secLvEl.Value;
                                }

                                // Link
                                entities.Add(new UrlEntity(link, text));
                            }
                            else
                            {
                                throw new NotImplementedException("Unknown link type.");
                            }
                        }
                        else if (secLvEl.Name == "span")
                        {
                            if (!secLvEl.HasAttributes)
                            {
                                entities.AddText(secLvEl.Value);
                            }
                            else if (secLvEl.Attribute("class")?.Value == mentionContainerClassName)
                            {
                                var anchorElement = secLvEl.Elements().First();
                                var anchorElementClassNames = anchorElement.Attribute("class")?.Value;

                                if (anchorElementClassNames == mentionAnchorClassName)
                                {
                                    var link = anchorElement.Attribute("href")?.Value ?? string.Empty;
                                    var text = anchorElement.Value;

                                    // Anchorlink
                                    entities.Add(new MentionEntity(text, text.Substring(1)));
                                }
                            }
                            else
                            {
                                throw new NotImplementedException("Unknown span type.");
                            }
                        }
                        else
                        {
                            throw new NotImplementedException("Unknown element: " + secLvEl.Name);
                        }
                    }
                }
            }

            return entities.ToArray();
        }
    }
}
