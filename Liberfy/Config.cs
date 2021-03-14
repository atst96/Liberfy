using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal static partial class Config
    {
        public static class Functions
        {
            /// <summary>
            /// タイムラインの読み込みを行うかどうかのフラグ
            /// </summary>
            public const bool IsLoadTimelineEnabled = true;
        }

        /// <summary>
        /// キー管理APIの構成
        /// </summary>
        public static partial class KeyManager
        {
            /// <summary>
            /// APIのエンドポイント
            /// </summary>
            public static readonly Uri ApiEndpoint = new("YOUR_KEY_MANAGEMENT_API_ENDPOINT");
        }

        /// <summary>
        /// Twitter関連の設定
        /// </summary>
        public static partial class Twitter
        {
            /// <summary>
            /// ConsumerKey
            /// </summary>
            public const string @ConsumerKey = "YOUR_TWITTER_REST_API_CONSUMER_KEY";

            /// <summary>
            /// ConsumerSecret
            /// </summary>
            public const string @ConsumerSecret = "YOUR_TWITTER_REST_API_CONSUMER_SECRET";
        }

        /// <summary>
        /// Mastodon関連の設定
        /// </summary>
        public static partial class Mastodon
        {
        }
    }
}
