using System.Collections.Generic;

namespace ToriatamaText.InternalExtractors
{
    using static Utils;

    static class CashtagExtractor
    {
        private static bool IsSymbolAfterCashtag(char c)
        {
            return c < AsciiTableLength && (AsciiTable[c] & CharType.SymbolAfterCashtag) != 0;
        }

        public static void Extract(string text, List<EntityInfo> result)
        {
            var startIndex = 0;

            Start:
            if (startIndex >= text.Length - 1) return;

            var dollarIndex = text.IndexOf('$', startIndex);
            if (dollarIndex == -1 || dollarIndex == text.Length - 1) return;

            var nextIndex = dollarIndex + 1;

            // 1文字目をチェック
            if (!IsAlphabet(text[nextIndex++]))
                goto GoToNextIndex;

            // 1文字前をチェック
            if (dollarIndex > 0 && !IsUnicodeSpace(text[dollarIndex - 1]))
                goto GoToNextIndex;

            // 残り5文字
            var i = 0;
            for (; nextIndex < text.Length; nextIndex++)
            {
                var c = text[nextIndex];
                if (IsAlphabet(c))
                {
                    if (i < 5)
                    {
                        i++;
                        continue;
                    }
                    goto GoToNextIndex;
                }

                if (c == '.' || c == '_')
                {
                    if (nextIndex + 1 < text.Length && IsAlphabet(text[nextIndex + 1]))
                    {
                        if (nextIndex + 2 < text.Length && IsAlphabet(text[nextIndex + 2]))
                        {
                            // '.', '_' のあとは2文字まで。オーバーしてたら '.', '_' の前まで
                            if (!(nextIndex + 3 < text.Length && IsAlphabet(text[nextIndex + 3])))
                                nextIndex += 3;
                        }
                        else
                        {
                            nextIndex += 2;
                        }
                    }
                    break;
                }

                if (IsSymbolAfterCashtag(c))
                    break;

                goto GoToNextIndex;
            }

            result.Add(new EntityInfo(dollarIndex, nextIndex - dollarIndex, EntityType.Cashtag));

            GoToNextIndex:
            startIndex = nextIndex;
            goto Start;
        }
    }
}
