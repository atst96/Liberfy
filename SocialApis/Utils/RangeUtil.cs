using System.Collections.Generic;

namespace SocialApis.Utils
{
    internal static class RangeUtil
    {
        /// <summary>
        /// <paramref name="start"/>から<paramref name="end"/>の範囲の数値を列挙する。
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <returns></returns>
        public static IEnumerable<byte> Enumerate(byte start, byte end)
        {
            if (start == end)
            {
                yield return start;
                yield break;
            }

            if (start < end)
            {
                byte value = start;
                do { yield return value; }
                while (value != byte.MaxValue && ++value <= end);
            }
            else
            {
                byte value = start;
                do { yield return value; }
                while (value != byte.MinValue && end <= --value);
            }
        }
    }
}
