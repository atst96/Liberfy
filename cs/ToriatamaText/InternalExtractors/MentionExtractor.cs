using System.Collections.Generic;

namespace ToriatamaText.InternalExtractors
{
    using static Utils;

    static class MentionExtractor
    {
        private static readonly char[] AtSigns = { '@', '＠' };

        private static bool IsPrecedingChar(string text, int index)
        {
            if (index < 0) return true;

            var c = text[index];
            if (c < AsciiTableLength)
            {
                if ((AsciiTable[c] & (CharType.Alnum | CharType.MentionNotPrecedingSymbol)) == 0)
                    return true;

                if ((c == 'T' || c == 't') && index > 0)
                {
                    c = text[index - 1];
                    return c == 'R' || c == 'r';
                }
                return false;
            }

            return true;
        }

        public static void Extract(string text, bool includeList, List<EntityInfo> result)
        {
            var startIndex = 0;

            Start:
            if (startIndex >= text.Length - 1) return;

            var atIndex = text.IndexOfAny(AtSigns, startIndex);
            if (atIndex == -1 || atIndex == text.Length - 1) return;

            // @ の連続を処理
            var nextIndex = atIndex + 1;
            while (true)
            {
                if (nextIndex == text.Length) return;
                var c = text[nextIndex++];
                if (c < AsciiTableLength)
                {
                    if ((AsciiTable[c] & (CharType.Alnum | CharType.ScreenNameSymbol)) != 0)
                        break;
                    if (c != '@')
                        goto GoToNextIndex;
                }
                else if (c != '＠')
                {
                    goto GoToNextIndex;
                }
            }

            // 1文字前をチェック
            if (!IsPrecedingChar(text, atIndex - 1))
                goto GoToNextIndex;

            atIndex = nextIndex - 2;

            // 残り 19 文字
            {
                var i = 0;
                while (true)
                {
                    if (nextIndex == text.Length)
                    {
                        result.Add(new EntityInfo(atIndex, nextIndex - atIndex, EntityType.Mention));
                        return;
                    }

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
                            goto GoToNextIndex;
                        }

                        if (c == '@' || (c == ':' && nextIndex + 1 < text.Length && text[nextIndex] == '/' && text[nextIndex + 1] == '/'))
                            goto GoToNextIndex;

                        if (c == '/' && nextIndex < text.Length && IsAlphabet(text[nextIndex]))
                        {
                            nextIndex++;
                            break; // リストへ
                        }
                    }
                    else {
                        if (c == '＠' || IsAccentChar(c))
                            goto GoToNextIndex;
                    }

                    result.Add(new EntityInfo(atIndex, nextIndex - atIndex - 1, EntityType.Mention));
                    goto GoToNextIndex;
                }
            }

            // @screenname/（現在地）listslug
            // 残り 24 文字
            if (includeList)
            {
                var i = 0;
                while (true)
                {
                    if (nextIndex == text.Length)
                    {
                        result.Add(new EntityInfo(atIndex, nextIndex - atIndex, EntityType.Mention));
                        return;
                    }

                    var c = text[nextIndex++];
                    if (c < AsciiTableLength)
                    {
                        if ((AsciiTable[c] & (CharType.Alnum | CharType.ListSlugSymbol)) != 0)
                        {
                            if (i < 24)
                            {
                                i++;
                                continue;
                            }
                            goto GoToNextIndex;
                        }

                        if (c == '@')
                            goto GoToNextIndex;

                        if (c == ':' && nextIndex + 1 < text.Length && text[nextIndex] == '/' && text[nextIndex + 1] == '/')
                        {
                            startIndex = nextIndex + 2;
                            goto Start;
                        }
                    }
                    else
                    {
                        if (c == '＠' || IsAccentChar(c))
                            goto GoToNextIndex;
                    }

                    result.Add(new EntityInfo(atIndex, nextIndex - atIndex - 1, EntityType.Mention));
                    goto GoToNextIndex;
                }
            }

            GoToNextIndex:
            startIndex = nextIndex;
            goto Start;
        }
    }
}
