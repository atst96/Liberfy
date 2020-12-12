#nullable enable

using MessagePack;

namespace Liberfy.Settings
{
    [Union(0, typeof(TwitterAccountSetting))]
    [Union(1, typeof(MastodonAccountSetting))]
    internal interface IAccountSetting
    {
        /// <summary>
        /// 内部ユーザID
        /// </summary>
        public string ItemId { get; }
    }
}
