using Liberfy.ViewModel;
using MessagePack;
using SocialApis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Liberfy.Settings
{
    [DataContract]
    internal class AccountSetting : NotificationObject
    {
        [Key("accounts")]
        [DataMember(Name = "accounts")]
        public IEnumerable<AccountItem> Accounts { get; set; }

        [Key("columns")]
        [DataMember(Name = "columns")]
        public IEnumerable<ColumnSetting> Columns { get; set; }
    }
}
