using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialApis.Utils
{
    /// <summary>
    /// Web関連のUtilクラス
    /// </summary>
    internal static class WebUtil
    {
        /// <summary>
        /// 文字列をパーセントエンコードする。
        /// </summary>
        /// <param name="content">エンコードする文字列</param>
        /// <returns>パーセントエンコード済み文字列</returns>
        public static string UrlEncode(string content)
            => HttpUtility.UrlEncode(content, EncodingUtil.UTF8);

        /// <summary>
        /// 文字列をパーセントエンコードする。
        /// </summary>
        /// <param name="values"><see cref="KeyValuePair{String, String}"/></param>
        /// <returns><see cref="KeyValuePair{String, String}"/></returns>
        public static IEnumerable<KeyValuePair<string, string>> UrlEncode(this IEnumerable<KeyValuePair<string, string>> values)
            => values.Select(kvp => new KeyValuePair<string, string>(UrlEncode(kvp.Key), UrlEncode(kvp.Value)));
    }
}
