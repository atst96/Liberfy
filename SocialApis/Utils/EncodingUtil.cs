using System.Text;

namespace SocialApis.Utils
{
    /// <summary>
    /// 文字エンコードに関するUtilクラス
    /// </summary>
    internal static class EncodingUtil
    {
        /// <summary>
        /// ASCII
        /// </summary>
        public static Encoding ASCII { get; } = new ASCIIEncoding();

        /// <summary>
        /// UTF-8(BOMなし)
        /// </summary>
        public static Encoding UTF8 { get; } = new UTF8Encoding(false);

        /// <summary>
        /// UTF-8(BOM付き)
        /// </summary>
        public static Encoding UTF8BOM { get; } = new UTF8Encoding(true);
    }
}
