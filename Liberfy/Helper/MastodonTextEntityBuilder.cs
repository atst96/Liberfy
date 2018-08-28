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

namespace Liberfy
{
    internal class MastodonTextEntityBuilder : ITextEntityBuilder
    {
        private static Regex _brTagReplaceRegex { get; } = new Regex("<br>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private string _content;

        public MastodonTextEntityBuilder(string content)
        {
            this._content = content;
        }

        public static void AddText(LinkedList<IEntity> entities, string text)
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
                        AddText(entities, "\n");
                    }

                    foreach (var secLvNode in topParagraphElement.Nodes())
                    {
                        if (secLvNode is XText secLvTextNode)
                        {
                            var value = secLvTextNode.Value;
                            AddText(entities, value);
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
