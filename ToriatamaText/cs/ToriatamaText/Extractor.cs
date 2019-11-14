using System;
using System.Collections.Generic;
using ToriatamaText.InternalExtractors;

namespace ToriatamaText
{
    public class Extractor
    {
        private readonly Dictionary<int, TldInfo> _tldDictionary = new Dictionary<int, TldInfo>();
        private int _longestTldLength;
        private int _shortestTldLength = int.MaxValue;

        public bool ExtractsUrlWithoutProtocol { get; set; } = true;

        public Extractor(IEnumerable<string> gTlds, IEnumerable<string> ccTlds)
        {
            if (gTlds != null)
                foreach (var x in gTlds) this.AddTld(x, TldType.GTld);

            if (ccTlds != null)
            {
                foreach (var x in ccTlds)
                {
                    if (!x.Equals("co", StringComparison.OrdinalIgnoreCase) && !x.Equals("tv", StringComparison.OrdinalIgnoreCase))
                        this.AddTld(x, TldType.CcTld);
                }
            }

            this.AddTld("co", TldType.SpecialCcTld);
            this.AddTld("tv", TldType.SpecialCcTld);
        }

        public Extractor() : this(DefaultTlds.GTlds, DefaultTlds.CTlds) { }

        private void AddTld(string tld, TldType type)
        {
            var len = tld.Length;
            if (len > this._longestTldLength)
                this._longestTldLength = len;
            else if (len < this._shortestTldLength)
                this._shortestTldLength = len;

            this._tldDictionary.Add(Utils.CaseInsensitiveHashCode(tld), new TldInfo(type, len));
            // ハッシュが被ったら知らん
        }

        public List<EntityInfo> ExtractEntities(string text)
        {
            var result = new List<EntityInfo>();
            if (!string.IsNullOrEmpty(text))
            {
                UrlExtractor.Extract(text, this.ExtractsUrlWithoutProtocol, this._tldDictionary, this._longestTldLength, this._shortestTldLength, result);
                HashtagExtractor.Extract(text, result);
                MentionExtractor.Extract(text, true, result);
                CashtagExtractor.Extract(text, result);
                RemoveOverlappingEntities(result);
            }
            return result;
        }

        public List<EntityInfo> ExtractMentionedScreenNames(string text)
        {
            var result = new List<EntityInfo>();
            if (!string.IsNullOrEmpty(text))
                MentionExtractor.Extract(text, false, result);
            return result;
        }

        public List<EntityInfo> ExtractMentionsOrLists(string text)
        {
            var result = new List<EntityInfo>();
            if (!string.IsNullOrEmpty(text))
                MentionExtractor.Extract(text, true, result);
            return result;
        }

        public string ExtractReplyScreenName(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            return ReplyExtractor.Extract(text);
        }

        public List<EntityInfo> ExtractUrls(string text)
        {
            var result = new List<EntityInfo>();
            if (!string.IsNullOrEmpty(text))
                UrlExtractor.Extract(text, this.ExtractsUrlWithoutProtocol, this._tldDictionary, this._longestTldLength, this._shortestTldLength, result);
            return result;
        }

        public List<EntityInfo> ExtractHashtags(string text, bool checkUrlOverlap)
        {
            var result = new List<EntityInfo>();
            if (!string.IsNullOrEmpty(text))
            {
                HashtagExtractor.Extract(text, result);

                if (checkUrlOverlap && result.Count > 0)
                {
                    UrlExtractor.Extract(text, this.ExtractsUrlWithoutProtocol, this._tldDictionary, this._longestTldLength, this._shortestTldLength, result);
                    RemoveOverlappingEntities(result);
                    result.RemoveAll(x => x.Type != EntityType.Hashtag);
                }
            }
            return result;
        }

        public List<EntityInfo> ExtractCashtags(string text)
        {
            var result = new List<EntityInfo>();
            if (!string.IsNullOrEmpty(text))
                CashtagExtractor.Extract(text, result);
            return result;
        }

        private static void RemoveOverlappingEntities(List<EntityInfo> entities)
        {
            if (entities.Count <= 1) return;

            entities.Sort((x, y) => x.StartIndex - y.StartIndex);

            var prevEnd = entities[0].StartIndex + entities[0].Length;
            var i = 1;
            while (i < entities.Count)
            {
                var current = entities[i];
                if (prevEnd > current.StartIndex)
                {
                    entities.RemoveAt(i); // これ遅そう
                }
                else
                {
                    prevEnd = current.StartIndex + current.Length;
                    i++;
                }
            }
        }
    }
}
