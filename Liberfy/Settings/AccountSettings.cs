using Liberfy.Components.JsonFormatters;
using Liberfy.ViewModels;
using SocialApis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Liberfy.Settings
{
    [DataContract]
    internal class AccountSettings : NotificationObject
    {
        [DataMember(Name = "accounts")]
        [Utf8Json.JsonFormatter(typeof(AccountSettingsIEnumerableFormatter))]
        public IEnumerable<AccountSettingBase> Accounts { get; set; }

        [DataMember(Name = "columns")]
        public IEnumerable<ColumnSetting> Columns { get; set; }
    }
}
