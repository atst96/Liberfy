using System.Collections.Generic;
using ToriatamaText.Collections;

namespace ToriatamaText.InternalExtractors
{
    using static Utils;

    static class UrlExtractor
    {
        private static bool IsValidDomainChar(char c)
        {
            return c < AsciiTableLength
                ? (AsciiTable[c] & (CharType.Alnum | CharType.DomainSymbol)) != 0
                : !(
                    (c >= '\u2000' && c <= '\u206F') // General Punctuation
                    || c == '\u00A0' || c == '\u1680' || c == '\u3000' // Category 'Z'
                );
        }

        private static bool IsUnicodeDomainChar(char c)
        {
            return c > 0x7F && !IsAccentChar(c);
        }

        private static bool IsAlnumAt(char c)
        {
            return c < AsciiTableLength && (AsciiTable[c] & CharType.AlnumAt) != 0;
        }

        private static bool IsAlnum(char c)
        {
            return c < AsciiTableLength && (AsciiTable[c] & CharType.Alnum) != 0;
        }

        private static bool IsPrecedingChar(char c)
        {
            return c < AsciiTableLength
                ? (AsciiTable[c] & (CharType.Alnum | CharType.UrlNotPrecedingSymbol)) == 0
                : !(c == '＠' || (c >= '\u202A' && c <= '\u202E'));
        }

        private static bool IsCyrillicScript(char c)
        {
            return (c >= '\u0400' && c <= '\u052F')
                || (c >= '\u2DE0' && c <= '\u2DFF') || (c >= '\uA640' && c <= '\uA69F')
                || c == '\u1D2B' || c == '\u1D78' || c == '\uFE2E' || c == '\uFE2F';
        }

        private static int EatPath(string text, int startIndex)
        {
            var lastEndingCharIndex = -1;
            var lastParenStartIndex = -1;
            var lastLengthInParen = 0;

            for (var i = startIndex; i < text.Length; i++)
            {
                var c = text[i];
                if (c < AsciiTableLength)
                {
                    switch (AsciiTable[c] & (CharType.Alnum | CharType.PathEndingSymbol | CharType.PathSymbol))
                    {
                        case 0:
                            if (c == '(')
                            {
                                lastLengthInParen = EatPathInParen(text, i + 1);
                                if (lastLengthInParen == 0)
                                    goto BreakLoop;
                                lastParenStartIndex = i;
                                i += lastLengthInParen;
                                break;
                            }
                            goto BreakLoop;
                        case CharType.PathSymbol:
                            break;
                        default:
                            lastEndingCharIndex = i;
                            break;
                    }
                }
                else if (IsCyrillicScript(c) || IsAccentChar(c))
                {
                    lastEndingCharIndex = i;
                }
                else
                {
                    goto BreakLoop;
                }
            }

            BreakLoop:
            if ((lastEndingCharIndex == -1 && lastParenStartIndex == startIndex) // 「a.com/(a)」などに対応
                || (lastEndingCharIndex + 1 == lastParenStartIndex)) // 「twitter.com/test(a).」は「twitter.com/test(a)」まで
            {
                lastEndingCharIndex = lastParenStartIndex + lastLengthInParen;
            }

            return lastEndingCharIndex == -1 ? 0 : lastEndingCharIndex - startIndex + 1;
        }

        private static int EatPathInParen(string text, int startIndex)
        {
            for (var i = startIndex; i < text.Length; i++)
            {
                var c = text[i];
                if (c < AsciiTableLength)
                {
                    if ((AsciiTable[c] & (CharType.Alnum | CharType.PathEndingSymbol | CharType.PathSymbol)) == 0)
                    {
                        if (c == '(')
                        {
                            var lengthInParen = EatPathInParen(text, i + 1);
                            if (lengthInParen == 0)
                                break;
                            i += lengthInParen;
                        }
                        else if (c == ')')
                        {
                            return i - startIndex + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (!IsCyrillicScript(c) && !IsAccentChar(c))
                {
                    break;
                }
            }

            return 0;
        }

        private static int EatQuery(string text, int startIndex)
        {
            var lastEndingCharIndex = -1;

            for (var i = startIndex; i < text.Length; i++)
            {
                var c = text[i];
                if (c >= AsciiTableLength) break;
                switch (AsciiTable[c] & (CharType.Alnum | CharType.QueryEndingSymbol | CharType.QuerySymbol))
                {
                    case 0:
                        goto BreakLoop;
                    case CharType.QuerySymbol:
                        break;
                    default:
                        lastEndingCharIndex = i;
                        break;
                }
            }

            BreakLoop:
            return lastEndingCharIndex == -1 ? 0 : lastEndingCharIndex - startIndex + 1;
        }

        public static void Extract(string text, bool urlWithoutProtocol, Dictionary<int, TldInfo> tldDic, int longestTldLength, int shortestTldLength, List<EntityInfo> result)
        {
            var dots = new MiniList<int>();
            var hashCodes = new MiniList<int>();
            var startIndex = 0;

            Start:
            if (startIndex >= text.Length - 2) return;

            var dotIndex = text.IndexOf('.', startIndex);
            if (dotIndex == -1 || dotIndex == text.Length - 1) return;
            if (dotIndex == startIndex)
            {
                // 開始位置にいきなり . があったら正しい URL なわけないでしょ
                goto GoToNextToDot;
            }

            // dotIndex の位置
            // www.(←)twitter.com/
            // twitter.(←)com/

            // . の前後が - や _ なら終了
            var x = text[dotIndex - 1];
            if (x == '-' || x == '_')
                goto GoToNextToDot;
            x = text[dotIndex + 1];
            if (x == '-' || x == '_')
                goto GoToNextToDot;

            // 前方向に探索
            // PrecedingChar まで戻る
            var precedingIndex = -1;
            var lastUnicodeCharIndex = -1;
            var hasScheme = false;
            for (var i = dotIndex - 1; i >= startIndex; i--)
            {
                var c = text[i];

                if (c == '/')
                {
                    // ホストの最初が - や _ なら終了
                    x = text[i + 1];
                    if (x == '-' || x == '_')
                        goto GoToNextToDot;

                    // スキーム判定
                    if (i >= 6)
                    {
                        var j = i - 1;
                        if (text[j--] == '/' && text[j--] == ':')
                        {
                            switch (ToLower(text[j--]))
                            {
                                case 's':
                                    if (i >= 7 && ToLower(text[j--]) == 'p')
                                        goto case 'p';
                                    break;
                                case 'p':
                                    if (ToLower(text[j--]) == 't' && ToLower(text[j--]) == 't' && ToLower(text[j--]) == 'h')
                                    {
                                        if (j < 0 || IsPrecedingChar(text[j]))
                                        {
                                            precedingIndex = j;
                                            hasScheme = true;
                                            goto BreakSchemeCheck;
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    goto GoToNextToDot;
                }

                if (!IsValidDomainChar(c))
                {
                    if (IsPrecedingChar(c))
                    {
                        precedingIndex = i;
                        break;
                    }

                    // PrecedingChar でないなら無効
                    goto GoToNextToDot;
                }

                if (lastUnicodeCharIndex == -1 && IsUnicodeDomainChar(c))
                    lastUnicodeCharIndex = i;
            }

            if (!urlWithoutProtocol)
                goto GoToNextToDot;

            if (lastUnicodeCharIndex != -1)
            {
                if (lastUnicodeCharIndex != dotIndex - 1 && IsPrecedingChar(text[lastUnicodeCharIndex]))
                {
                    // Unicode文字を含まないようにして救済
                    precedingIndex = lastUnicodeCharIndex;
                    lastUnicodeCharIndex = -1;
                }
                else
                {
                    goto GoToNextToDot;
                }
            }

            x = text[precedingIndex + 1];
            if ((precedingIndex == -1 && startIndex != 0) || x == '-' || x == '_')
                goto GoToNextToDot;

            BreakSchemeCheck:
            // ホスト部分を最後まで読み取る
            dots.Clear();
            dots.Add(dotIndex + 1);
            var hasUnicodeCharAfterDot = false;
            var nextIndex = text.Length;
            for (var i = dotIndex + 1; i < text.Length; i++)
            {
                var c = text[i];

                if (c == '.')
                {
                    // . が text の最後なら終了
                    // スキームなしなのに Unicode 文字が含まれていたら終了
                    if (i == text.Length - 1 || (!hasScheme && hasUnicodeCharAfterDot))
                    {
                        nextIndex = i;
                        break;
                    }

                    // . の前後の文字が - や _ なら終了
                    x = text[i - 1];
                    if (x == '-' || x == '_')
                    {
                        nextIndex = i - 1;
                        break;
                    }
                    x = text[i + 1];
                    if (x == '-' || x == '_')
                    {
                        nextIndex = i;
                        break;
                    }

                    dots.Add(i + 1);
                    continue;
                }

                if (!IsValidDomainChar(c))
                {
                    nextIndex = i;
                    break;
                }

                if (!hasUnicodeCharAfterDot)
                    hasUnicodeCharAfterDot = IsUnicodeDomainChar(c);
            }

            // TLD 検証
            TldInfo tldInfo;
            int dotCount;
            for (var i = dots.Count - 1; i >= 0; i--)
            {
                var dotIndexPlusOne = dots[i];
                var len = nextIndex - dotIndexPlusOne;
                if (len < shortestTldLength) continue;
                if (len > longestTldLength) len = longestTldLength;
                nextIndex = dotIndexPlusOne + len;

                // ループ回数軽減のため、その場でハッシュ値を求める
                hashCodes.Clear();
                var hash1 = 5381;
                var hash2 = hash1;

                for (var j = dotIndexPlusOne; j < nextIndex;)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ ToLower(text[j++]);
                    hashCodes.Add(hash1 + hash2 * 1566083941);
                    if (j >= nextIndex) break;
                    hash2 = ((hash2 << 5) + hash2) ^ ToLower(text[j++]);
                    hashCodes.Add(hash1 + hash2 * 1566083941);
                }

                for (var j = hashCodes.Count - 1; j >= 0; j--)
                {
                    nextIndex = dotIndexPlusOne + j + 1;
                    if ((nextIndex == text.Length || !IsAlnumAt(text[nextIndex]))
                        && tldDic.TryGetValue(hashCodes[j], out tldInfo)
                        && nextIndex - dotIndexPlusOne == tldInfo.Length) // ハッシュ衝突の簡易チェック
                    {
                        dotCount = i + 1;
                        goto TldDecided;
                    }
                }
            }

            goto GoToNextToDot;

            TldDecided:
            // ccTLD のサブドメインなしはスキーム必須
            if (!hasScheme && tldInfo.Type == TldType.CcTld
                && (dotCount == 1 && (nextIndex >= text.Length || text[nextIndex] != '/')))
                goto GoToNextIndex;

            // サブドメインには _ を使えるがドメインには使えない
            for (var i = dots.Last - 2; i > precedingIndex; i--)
            {
                var c = text[i];
                if (c == '.' || c == '/') break;
                if (c == '_')
                    goto GoToNextIndex;
            }

            var urlStartIndex = precedingIndex + 1;

            if (nextIndex >= text.Length)
                goto AddAndGoNext;

            // ポート番号
            if (text[nextIndex] == ':' && ++nextIndex < text.Length)
            {
                var portNumberLength = 0;
                for (; nextIndex < text.Length; nextIndex++)
                {
                    var c = text[nextIndex];
                    if (c <= '9' && c >= '0')
                        portNumberLength++;
                    else
                        break;
                }

                if (portNumberLength == 0)
                {
                    result.Add(new EntityInfo(urlStartIndex, (--nextIndex) - urlStartIndex, EntityType.Url));
                    goto GoToNextIndex;
                }
            }

            if (nextIndex >= text.Length)
                goto AddAndGoNext;

            // パス
            if (text[nextIndex] == '/')
            {
                // https?://t.co/xxxxxxxxxx だけ特別扱い
                var len = nextIndex - urlStartIndex;
                nextIndex++;
                if (hasScheme && (len == 11 || len == 12)
                    && ToLower(text[nextIndex - 2]) == 'o' && ToLower(text[nextIndex - 3]) == 'c'
                    && text[nextIndex - 4] == '.' && ToLower(text[nextIndex - 5]) == 't' && text[nextIndex - 6] == '/'
                    && nextIndex < text.Length && IsAlnum(text[nextIndex]))
                {
                    nextIndex++;
                    for (; nextIndex < text.Length; nextIndex++)
                    {
                        if (!IsAlnum(text[nextIndex]))
                            break;
                    }
                    goto AddAndGoNext;
                }

                nextIndex += EatPath(text, nextIndex);
            }

            if (nextIndex >= text.Length)
                goto AddAndGoNext;

            // クエリ
            if (text[nextIndex] == '?')
            {
                nextIndex++;
                nextIndex += EatQuery(text, nextIndex);
            }

            AddAndGoNext:
            result.Add(new EntityInfo(urlStartIndex, nextIndex - urlStartIndex, EntityType.Url));

            GoToNextIndex:
            startIndex = nextIndex;
            goto Start;

            GoToNextToDot:
            startIndex = dotIndex + 1;
            goto Start;
        }
    }
}
