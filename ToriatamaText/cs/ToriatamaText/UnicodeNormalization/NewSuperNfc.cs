using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ToriatamaText.Collections;

namespace ToriatamaText.UnicodeNormalization
{
    using static Tables;

    public static class NewSuperNfc
    {
        private const int SBase = 0xAC00,
            LBase = 0x1100, VBase = 0x1161, TBase = 0x11A7,
            LCount = 19, VCount = 21, TCount = 28,
            NCount = VCount * TCount,
            SCount = LCount * NCount;

        /// <returns><c>true</c>なら<param name="result" />に結果が入る。<c>false</c>なら既に正規化されている。</returns>
        public static bool Compose(string s, out MiniList<char> result)
        {
            // クイックチェック
            bool isFirstCharToNormalizeSurrogatePair;
            var i = IndexOfLastNormalizedChar(s, 0, out isFirstCharToNormalizeSurrogatePair);

            if (i == -1)
            {
                result = new MiniList<char>();
                return false;
            }

            // ここからが本番
            result = StringToMiniList(s, i);

            while (true)
            {
                var nextQcYes = FindNextNfcQcYes(s, i + (isFirstCharToNormalizeSurrogatePair ? 2 : 1));
                var countBeforeDecompose = result.Count;

                DecomposeInRange(s, i, nextQcYes, ref result);
                ComposeInRange(ref result, countBeforeDecompose);

                if (nextQcYes == s.Length)
                    break;

                i = IndexOfLastNormalizedChar(s, nextQcYes + 1, out isFirstCharToNormalizeSurrogatePair);

                var len = (i == -1 ? s.Length : i) - nextQcYes;
                if (len > 0)
                {
                    result.EnsureCapacity(len);
                    s.CopyTo(nextQcYes, result.InnerArray, result.Count, len);
                    result.Count += len;
                }

                if (i == -1) break;
            }

            return true;
        }

        // char.IsHighSurrogate と使い分けると速度が変わってる（ような気がする）

        private static bool IsHighSurrogate(uint code)
        {
            return code >= 0xD800 && code <= 0xDBFF;
        }

        private static bool IsLowSurrogate(uint code)
        {
            return code >= 0xDC00 && code <= 0xDFFF;
        }

        private static uint ToUtf16Int(uint hi, uint lo)
        {
            return hi << 16 | lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ToUtf16Int(string s, int index, out bool isSurrogatePair)
        {
            uint hi = s[index];
            isSurrogatePair = IsHighSurrogate(hi) && index + 1 < s.Length && IsLowSurrogate(s[index + 1]);
            return isSurrogatePair ? ToUtf16Int(hi, s[index + 1]) : hi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ToUtf16Int(char[] c, int index, out bool isSurrogatePair)
        {
            uint hi = c[index];
            isSurrogatePair = IsHighSurrogate(hi) && index + 1 < c.Length && IsLowSurrogate(c[index + 1]);
            return isSurrogatePair ? ToUtf16Int(hi, c[index + 1]) : hi;
        }

        private static int IndexOfLastNormalizedChar(string s, int startIndex, out bool isFirstCharToNormalizeSurrogatePair)
        {
            var i = startIndex;
            while (i < s.Length)
            {
                bool isSurrogatePair;
                var x = ToUtf16Int(s, i, out isSurrogatePair);

                // CccAndQcTable にキーが存在する
                // → CCC が 0 ではない、または NFC_QC が YES ではない
                if (LookupCccAndQcTable(x))
                {
                    if (i > 0)
                    {
                        isFirstCharToNormalizeSurrogatePair = i >= 2 && char.IsHighSurrogate(s[i - 2]) && char.IsLowSurrogate(s[i - 1]);
                        i -= isFirstCharToNormalizeSurrogatePair ? 2 : 1;
                    }
                    else
                    {
                        isFirstCharToNormalizeSurrogatePair = isSurrogatePair;
                    }
                    return i;
                }

                i += isSurrogatePair ? 2 : 1;
            }

            isFirstCharToNormalizeSurrogatePair = false;
            return -1;
        }

        private static MiniList<char> StringToMiniList(string s, int count)
        {
            var arr = new char[s.Length * 2];
            s.CopyTo(0, arr, 0, count);
            return new MiniList<char>
            {
                InnerArray = arr,
                Count = count
            };
        }

        private static int FindNextNfcQcYes(string s, int startIndex)
        {
            var i = startIndex;
            while (i < s.Length)
            {
                bool isSurrogatePair;
                var x = ToUtf16Int(s, i, out isSurrogatePair);
                if (!LookupCccAndQcTable(x))
                    break;
                i += isSurrogatePair ? 2 : 1;
            }
            return i;
        }

        private static void DecomposeInRange(string s, int startIndex, int endIndex, ref MiniList<char> dest)
        {
            var i = startIndex;
            while (i < endIndex)
            {
                bool isSurrogatePair;
                var x = ToUtf16Int(s, i, out isSurrogatePair);

                DecompCore(x, ref dest);

                i += isSurrogatePair ? 2 : 1;
            }
        }

        private static void DecompCore(uint code, ref MiniList<char> result)
        {
            // ハングルはどうせ合成するから分解しない

            // Unicode 8.0 用ハードコーディング
            // 10000 以上離れているところとハングルをショートカット
            if (!(code < 0x00C0 || (code > 0x1026 && (code < 0x1B06 || (code > 0x30FE && (code < 0xF900))))))
            {
                var i = LookupDecompositionTable(code);
                if (i != -1)
                {
                    var first = DecompositionTableEntries[i];
                    DecompCore(first, ref result);

                    var second = DecompositionTableEntries[i + 1];
                    if (second != 0)
                        DecompCore(second, ref result);

                    return;
                }
            }

            var insertIndex = result.Count;
            var isSurrogatePair = code > char.MaxValue;

            if (insertIndex > 0)
            {
                var ccc = GetCanonicalCombiningClass(code);
                if (ccc != 0)
                {
                    var j = insertIndex - 1;
                    while (true)
                    {
                        uint prev = result[j];
                        var isPrevSurrogatePair = IsLowSurrogate(prev) && j > 0 && IsHighSurrogate(result[j - 1]);
                        var prevCcc = GetCanonicalCombiningClass(isPrevSurrogatePair ? ToUtf16Int(result[--j], prev) : prev);
                        if (prevCcc <= ccc) break;
                        insertIndex = j;
                        if (j == 0)
                        {
                            insertIndex = 0;
                            break;
                        }
                        j--;
                    }
                }

                if (result.InnerArray.Length < result.Count + 2)
                {
                    var newArray = new char[result.Count * 2];
                    if (insertIndex < result.Count)
                    {
                        Array.Copy(result.InnerArray, newArray, insertIndex);
                        Array.Copy(result.InnerArray, insertIndex, newArray, insertIndex + (isSurrogatePair ? 2 : 1), result.Count - insertIndex);
                    }
                    else
                    {
                        Array.Copy(result.InnerArray, newArray, result.Count);
                    }
                    result.InnerArray = newArray;
                }
                else
                {
                    if (insertIndex < result.Count)
                        Array.Copy(result.InnerArray, insertIndex, result.InnerArray, insertIndex + (isSurrogatePair ? 2 : 1), result.Count - insertIndex);
                }
            }
            else
            {
                result.EnsureCapacity(2);
            }

            if (isSurrogatePair)
            {
                result.InnerArray[insertIndex] = (char)(code >> 16);
                result.InnerArray[insertIndex + 1] = (char)code;
                result.Count += 2;
            }
            else
            {
                result.InnerArray[insertIndex] = (char)code;
                result.Count++;
            }
        }

        private static void ComposeInRange(ref MiniList<char> list, int startIndex)
        {
            bool isLastSurrogatePair;
            uint last = ToUtf16Int(list.InnerArray, startIndex, out isLastSurrogatePair);
            var starterIndex = startIndex;
            var starter = ((ulong)last) << 32;
            var isStarterSurrogatePair = isLastSurrogatePair;
            var i = startIndex + (isLastSurrogatePair ? 2 : 1);
            var insertIndex = i;
            var lastCcc = 0;

            while (i < list.Count)
            {
                var hi = list[i];
                var isSurrogatePair = IsHighSurrogate(hi)
                    && i + 1 < list.Count && char.IsLowSurrogate(list[i + 1]);
                uint c;
                if (isSurrogatePair)
                {
                    c = ToUtf16Int(hi, list[i + 1]);
                    i += 2;
                }
                else
                {
                    c = hi;
                    i++;
                }

                // ハングル
                if (!isLastSurrogatePair && !isSurrogatePair) // このifあってる？？
                {
                    var LIndex = last - LBase;
                    if (LIndex >= 0 && LIndex < LCount)
                    {
                        var VIndex = c - VBase;
                        if (VIndex >= 0 && VIndex < VCount)
                        {
                            last = SBase + (LIndex * VCount + VIndex) * TCount;
                            list[insertIndex - 1] = (char)last;
                            lastCcc = 0;
                            continue;
                        }
                    }

                    var SIndex = last - SBase;
                    if (SIndex >= 0 && SIndex < SCount && (SIndex % TCount) == 0)
                    {
                        var TIndex = c - TBase;
                        if (0 < TIndex && TIndex < TCount)
                        {
                            last += TIndex;
                            list[insertIndex - 1] = (char)last;
                            lastCcc = 0;
                            continue;
                        }
                    }
                }
                // ハングルここまで

                var ccc = GetCanonicalCombiningClass(c);
                if (ccc != 0 && lastCcc == ccc)
                {
                    // ブロック
                    list[insertIndex++] = hi;
                    if (isSurrogatePair)
                        list[insertIndex++] = (char)c;
                    last = c;
                    isLastSurrogatePair = isSurrogatePair;
                    continue;
                }

                var key = starter | c;
                uint composed;
                if ((ccc != 0 || (ccc == 0 && lastCcc == 0)) && LookupCompositionTable(key, out composed))
                {
                    if (composed <= char.MaxValue)
                    {
                        if (isStarterSurrogatePair)
                        {
                            // 下位サロゲートのスペースを埋める
                            Debug.Assert(insertIndex < i);
                            for (var j = starterIndex + 1; j < --insertIndex; j++)
                                list[j] = list[j + 1];
                        }

                        list[starterIndex] = (char)composed;
                        isStarterSurrogatePair = false;
                    }
                    else
                    {
                        if (!isStarterSurrogatePair)
                        {
                            // 下位サロゲートを入れるスペースをつくる
                            Debug.Assert(insertIndex < i);
                            var starterLoIndex = starterIndex + 1;
                            for (var j = insertIndex; j > starterLoIndex; j--)
                                list[j] = list[j - 1];
                            insertIndex++;
                        }

                        list[starterIndex] = (char)(composed >> 16);
                        list[starterIndex + 1] = (char)(composed & char.MaxValue);
                        isStarterSurrogatePair = true;
                    }

                    starter = ((ulong)composed) << 32;
                    ccc = 0; // これでいい？？
                }
                else
                {
                    if (ccc == 0)
                    {
                        starterIndex = insertIndex;
                        starter = ((ulong)c) << 32;
                        isStarterSurrogatePair = isSurrogatePair;
                    }
                    list[insertIndex++] = hi;
                    if (isSurrogatePair)
                        list[insertIndex++] = (char)c;
                }

                last = c;
                isLastSurrogatePair = isSurrogatePair;
                lastCcc = ccc;
            }

            list.Count = insertIndex;
        }
    }
}
