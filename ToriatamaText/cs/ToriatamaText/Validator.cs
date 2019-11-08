using System;
using ToriatamaText.Collections;
using ToriatamaText.UnicodeNormalization;

namespace ToriatamaText
{
    public class Validator
    {
        private readonly Extractor _extractor;

        public int MaxTweetLength { get; set; } = 140;
        public int ShortUrlLength { get; set; } = 23;
        // we no longer need to separate ShortUrlLength and ShortUrlLengthHttps

        public Validator(Extractor extractor)
        {
            if (extractor == null)
                throw new ArgumentNullException(nameof(extractor));

            this._extractor = extractor;
        }

        public Validator() : this(new Extractor()) { }

        public int GetTweetLength(string text, bool normalize = true)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            if (normalize)
            {
                MiniList<char> normalized;
                if (NewSuperNfc.Compose(text, out normalized))
                    text = new string(normalized.InnerArray, 0, normalized.Count); // ポインタ使わせろ！！
            }

            var length = text.Length;

            foreach (var x in this._extractor.ExtractUrls(text))
                length += this.ShortUrlLength - x.Length;

            // サロゲートペアを削除
            var end = text.Length - 1;
            for (var i = 0; i < end;)
            {
                // char.IsSurrogatePair はインライン化されないじゃん？
                if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
                {
                    length--;
                    i += 2;
                }
                else
                {
                    i++;
                }
            }

            return length;
        }

        private static readonly char[] InvalidTweetChars =
        {
            '\uFFFE', '\uFEFF', '\uFFFF',
            '\u202A', '\u202B', '\u202C', '\u202D', '\u202E'
        };

        public bool IsValidTweet(string text, bool normalize = true)
        {
            if (string.IsNullOrEmpty(text) || text.IndexOfAny(InvalidTweetChars) != -1)
                return false;

            return this.GetTweetLength(text, normalize) <= this.MaxTweetLength;
        }
    }
}
