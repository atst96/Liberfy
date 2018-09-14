using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Liberfy.Model;
using Sgml;
using SocialApis.Mastodon;

namespace Liberfy
{
    internal class MastodonTextEntityBuilder : ITextEntityBuilder
    {
        private static Regex _brTagReplaceRegex { get; } = new Regex("<br>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private string _content;
        private Emoji[] _emojis;

        private static Regex _emojiRegex = new Regex(":(?<code>[a-zA-Z0-9_\\-]+):", RegexOptions.Multiline);

        public MastodonTextEntityBuilder(string content, Emoji[] emojis)
        {
            this._content = content;
            this._emojis = emojis;
        }

        private static void AddText(LinkedList<IEntity> entities, string text)
        {
            if (entities.Last?.Value is PlainTextEntity textEntity)
            {
                textEntity.DisplayText += text;
            }
            else
            {
                entities.AddLast(new PlainTextEntity(text));
            }
        }

        private static void AddText(LinkedList<IEntity> entities, string text, Emoji[] emojis)
        {
            if (emojis?.Length == 0)
            {
                AddText(entities, text);
                return;
            }

            var matches = _emojiRegex.Matches(text);
            if (matches.Count == 0)
            {
                AddText(entities, text);
                return;
            }

            if (matches[0].Index != 0)
            {
                AddText(entities, text.Substring(0, matches[0].Index));
            }

            for (int i = 0; i < matches.Count; ++i)
            {
                var m = matches[i];

                var shortCode = m.Groups["code"].Value;

                Emoji emoji = default;
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
                    AddText(entities, m.Value);
                }
                else
                {
                    entities.AddLast(new EmojiEntity(emoji));
                }

                if (i < matches.Count - 1)
                {
                    var nextMatch = matches[i + 1];
                    AddText(entities, text.Substring(m.Index + m.Value.Length, nextMatch.Index - m.Index - m.Value.Length));
                }
                else if (m.Index + m.Value.Length != text.Length)
                {
                    AddText(entities, text.Substring(m.Index + m.Value.Length));
                }
            }
        }

        public IEnumerable<IEntity> Build()
        {
            const string externalLinkRel = "nofollow noopener";

            const string hashtagElementClassName = "mention hashtag";

            const string mentionContainerClassName = "h-card";
            const string mentionAnchorClassName = "u-url mention";

            var content = _brTagReplaceRegex.Replace(this._content, "<br/>");

            var entities = new LinkedList<IEntity>();

            using (var sgmlReader = new SgmlReader
            {
                DocType = "html",
                IgnoreDtd = true,
                CaseFolding = CaseFolding.ToLower,
                InputStream = new StringReader($"<div>{ content }</div>"),
            })
            {
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
                        else if (secLvNode.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            var secLvEl = XElement.Load(secLvNode.CreateReader());

                            System.Diagnostics.Debug.WriteLine(secLvEl);

                            if (secLvEl.Name == "br")
                            {
                                AddText(entities, "\n");
                            }
                            else if (secLvEl.Name == "a")
                            {
                                var classNames = secLvEl.Attribute("class")?.Value;
                                var rels = secLvEl.Attribute("rel")?.Value;

                                if (classNames == hashtagElementClassName)
                                {
                                    var text = secLvEl.Value;
                                    // Hashtag
                                    entities.AddLast(new HashtagEntity(text));
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
                                    entities.AddLast(new UrlEntity(link, text));
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            else if (secLvEl.Name == "span")
                            {
                                if (secLvEl.Attribute("class")?.Value == mentionContainerClassName)
                                {
                                    var anchorElement = secLvEl.Elements().First();
                                    var anchorElementClassNames = anchorElement.Attribute("class")?.Value;

                                    if (anchorElementClassNames == mentionAnchorClassName)
                                    {
                                        var link = anchorElement.Attribute("href")?.Value ?? string.Empty;
                                        var text = anchorElement.Value;

                                        // Anchorlink
                                        entities.AddLast(new MentionEntity(text, text.Substring(1)));
                                    }
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                throw new NotImplementedException(secLvEl.Name.ToString());
                            }
                        }
                    }
                }
            }

            return entities.ToArray();
        }
    }
}
