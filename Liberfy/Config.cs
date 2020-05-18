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
        /// Twitter関連の設定
        /// </summary>
        public static partial class Twitter
        {
            //public const string @ConsumerKey = "";
            //public const string @ConsumerSecret = "";
        }

        /// <summary>
        /// Mastodon関連の設定
        /// </summary>
        public static partial class Mastodon
        {
        }
    }
}
