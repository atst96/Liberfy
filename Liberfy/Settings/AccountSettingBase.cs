using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Settings
{
    [DataContract]
    internal abstract class AccountSettingBase
    {
        protected AccountSettingBase() { }

        [DataMember(Name = "internal.id")]
        public string ItemId { get; set; }

        [DataMember(Name = "service")]
        public abstract ServiceType Service { get; }
    }
}
