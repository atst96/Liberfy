using System.Collections.Generic;
using System.Globalization;

namespace ToriatamaText.InternalExtractors
{
    static class HashtagExtractor
    {
        private static readonly char[] NumberSigns = { '#', '＃' };

        private static bool IsInHashtagLettersNumeralsSet(string text, int index, ref bool foundLetter)
        {
            // サロゲートペアに対応するため string とインデックスを引数に取る
            char c;
            switch (CharUnicodeInfo.GetUnicodeCategory(text, index))
            {
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.EnclosingMark:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                    foundLetter = true;
                    return true;
                case UnicodeCategory.DecimalDigitNumber:
                    return true;
                case UnicodeCategory.ConnectorPunctuation:
                    return text[index] == '_';
                case UnicodeCategory.Format:
                    c = text[index];
                    return c == '\u200c' || c == '\u200d';
                case UnicodeCategory.OtherPunctuation:
                    c = text[index];
                    return c == '\ua67e' || c == '\u05f3' || c == '\u05f4' || c == '\u30fb'
                        || c == '\u3003' || c == '\u0f0b' || c == '\u0f0c' || c == '\u00b7';
                case UnicodeCategory.DashPunctuation:
                    c = text[index];
                    return c == '\u05be' || c == '\u301c' || c == '\u30a0';
                case UnicodeCategory.MathSymbol:
                    return text[index] == '\uff5e';
                case UnicodeCategory.ModifierSymbol:
                    c = text[index];
                    return c == '\u309b' || c == '\u309c';
                default:
                    return false;
            }
        }

        public static void Extract(string text, List<EntityInfo> result)
        {
            var startIndex = 0;
            var _ = false;

            Start:
            if (startIndex >= text.Length - 1) return;

            var hashIndex = text.IndexOfAny(NumberSigns, startIndex);
            if (hashIndex == -1 || hashIndex == text.Length - 1) return;

            // カテゴリ L か M が1文字以上含まれる必要がある
            var foundLetter = false;
            var nextIndex = hashIndex + 1;

            // 1文字目をチェック
            {
                var c = text[nextIndex];
                if (c == '\uFE0F' || c == '\u20E3' || !IsInHashtagLettersNumeralsSet(text, nextIndex, ref foundLetter))
                    goto GoToNextIndex;

                nextIndex += char.IsHighSurrogate(c) ? 2 : 1;
            }

            // 1文字前をチェック
            if (hashIndex > 0 && (text[hashIndex - 1] == '&' || IsInHashtagLettersNumeralsSet(text, hashIndex - 1, ref _)))
                goto GoToNextIndex;

            // 残り
            while (nextIndex < text.Length)
            {
                if (!IsInHashtagLettersNumeralsSet(text, nextIndex, ref foundLetter))
                {
                    var c = text[nextIndex];
                    if (c == '#' || c == '＃')
                    {
                        startIndex = nextIndex + 1;
                        goto Start;
                    }
                    if (c == ':' && nextIndex + 2 < text.Length && text[nextIndex + 1] == '/' && text[nextIndex + 2] == '/')
                    {
                        startIndex = nextIndex + 3;
                        goto Start;
                    }
                    break;
                }

                nextIndex += char.IsHighSurrogate(text, nextIndex) ? 2 : 1;
            }

            if (foundLetter)
            {
                result.Add(new EntityInfo(hashIndex, nextIndex - hashIndex, EntityType.Hashtag));
            }

            GoToNextIndex:
            startIndex = nextIndex;
            goto Start;
        }
    }
}
