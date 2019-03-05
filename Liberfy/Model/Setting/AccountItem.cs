using Liberfy.ViewModels;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Settings
{
    /// <summary>
    /// アカウント設定をJsonデータに変換するためのクラス
    /// </summary>
    [DataContract]
    internal class AccountItem : IEquatable<AccountItem>
    {
        public AccountItem() { }

        [Key("service")]
        [DataMember(Name = "service")]
        public ServiceType Service { get; set; } = ServiceType.Twitter;

        [Key("user_id")]
        [DataMember(Name = "user_id")]
        public long Id { get; set; }

        [Key("screen_name")]
        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [Key("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [Key("profile_image_url")]
        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [Key("is_protected")]
        [DataMember(Name = "is_protected")]
        public bool IsProtected { get; set; }

        [Key("token")]
        [DataMember(Name = "token")]
        public ApiTokenInfo Token { get; set; }

        [Key("tokens_third")]
        [DataMember(Name = "tokens_third")]
        public ApiTokenInfo[] ThirdPartyTokens { get; set; }

        [Key("columns")]
        [DataMember(Name = "columns")]
        private IEnumerable<ColumnSetting> _columns;
        [IgnoreDataMember]
        public IEnumerable<ColumnSetting> Columns
        {
            get => this._columns ?? Enumerable.Empty<ColumnSetting>();
            set => this._columns = value;
        }

        [Key("mute.ids")]
        [DataMember(Name = "muted_ids")]
        public long[] MutedIds { get; set; }

        public bool Equals(AccountItem item)
        {
            return this.Service == item.Service && this.Id == item.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is AccountItem item ? this.Equals(item) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
