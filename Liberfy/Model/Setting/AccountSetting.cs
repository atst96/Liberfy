using Liberfy.ViewModel;
using SocialApis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Liberfy.Settings
{
    [DataContract]
    internal class AccountSetting : NotificationObject
    {
        [DataMember(Name = "accounts")]
        public IEnumerable<AccountItem> Accounts { get; set; }

        [DataMember(Name = "columns")]
        public IEnumerable<ColumnSetting> Columns { get; set; }
    }
}
