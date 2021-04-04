using System;
using System.Collections.Generic;
using MessagePack;

namespace Liberfy.Data.Mastodon
{
    /// <summary>
    /// インスタンス情報
    /// </summary>
    [MessagePackObject]
    internal class InstanceInfo
    {
        /// <summary>
        /// URL
        /// </summary>
        [Key("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// インスタンス名
        /// </summary>
        [Key("title")]
        public string Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [Key("description")]
        public string Description { get; set; }

        /// <summary>
        /// アイコン
        /// </summary>
        [Key("icon")]
        public Uri Icon { get; set; }

        /// <summary>
        /// ストリーミングAPIのURL
        /// </summary>
        [Key("urls.streamin_api")]
        public string StreamingApiEndpoint { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        [Key("version")]
        public string Version { get; set; }

        /// <summary>
        /// 言語
        /// </summary>
        [Key("languages")]
        public string[] Languages { get; set; }
    }
}
