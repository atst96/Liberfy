using Liberfy.ViewModels;
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

        [DataMember(Name = "service")]
        public ServiceType Service { get; set; } = ServiceType.Twitter;

        [DataMember(Name = "user_id")]
        public long Id { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name = "is_protected")]
        public bool IsProtected { get; set; }

        [DataMember(Name = "token")]
        public ApiTokenInfo Token { get; set; }

        [DataMember(Name = "tokens_third")]
        public ApiTokenInfo[] ThirdPartyTokens { get; set; }

        [DataMember(Name = "columns")]
        private IEnumerable<ColumnSetting> _columns;
        [IgnoreDataMember]
        public IEnumerable<ColumnSetting> Columns
        {
            get => this._columns ?? Enumerable.Empty<ColumnSetting>();
            set => this._columns = value;
        }

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
