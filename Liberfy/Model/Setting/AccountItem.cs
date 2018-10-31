using Liberfy.ViewModel;
using SocialApis;
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
        public SocialService Service { get; set; } = SocialService.Twitter;

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

        [DataMember(Name = "automatically_login")]
        public bool AutomaticallyLogin { get; set; }

        [DataMember(Name = "automatically_load_timeline")]
        public bool AutomaticallyLoadTimeline { get; set; }

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
