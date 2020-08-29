using System;
using System.Collections.Generic;
using System.Text;

namespace SocialApis.Mastodon
{
    public static class StreamEventTypes
    {
        /// <summary>
        /// 新規トゥート
        /// </summary>
        public const string Update = "update";

        /// <summary>
        /// 通知
        /// </summary>
        public const string Notification = "notification";

        /// <summary>
        /// 削除
        /// </summary>
        public const string Delete = "delete";

        /// <summary>
        /// キーワードフィルター変更
        /// </summary>
        public const string FiltersChanged = "filters_changed";
    }
}
