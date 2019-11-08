namespace ToriatamaText.InternalExtractors
{
    using static Utils;

    static class ReplyExtractor
    {
        public static string Extract(string text)
        {
            var startIndex = 0;

            while (true)
            {
                var c = text[startIndex++];
                if (c == '@' || c == '＠')
                {
                    if (startIndex < text.Length)
                    {
                        c = text[startIndex];
                        if (c < AsciiTableLength && (AsciiTable[c] & (CharType.Alnum | CharType.ScreenNameSymbol)) != 0)
                            break;
                    }
                    return null;
                }

                // whitespace チェック
                if (IsUnicodeSpace(c))
                {
                    if (text.Length == startIndex) return null;
                    continue;
                }

                return null;
            }

            var nextIndex = startIndex + 1;
            var i = 0;
            while (true)
            {
                if (nextIndex == text.Length)
                    return text.Substring(startIndex);

                var c = text[nextIndex++];
                if (c < AsciiTableLength)
                {
                    if ((AsciiTable[c] & (CharType.Alnum | CharType.ScreenNameSymbol)) != 0)
                    {
                        if (i < 19)
                        {
                            i++;
                            continue;
                        }
                        return null;
                    }

                    if (c == '@' || (c == ':' && nextIndex + 1 < text.Length && text[nextIndex] == '/' && text[nextIndex + 1] == '/'))
                        return null;
                }
                else
                {
                    if (c == '＠' || IsAccentChar(c))
                        return null;
                }

                return text.Substring(startIndex, nextIndex - startIndex - 1);
            }
        }
    }
}
