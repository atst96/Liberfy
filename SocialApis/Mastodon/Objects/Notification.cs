using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Notification
    {
        [DataMember(Name = "id"), Utf8Json.JsonFormatter(typeof(Formatters.StringToLongFormatter))]
        public long Id { get; private set; }

        /// <summary>
        /// SocialApis.Mastodon.NotificationTypes.*
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; private set; }

        [DataMember(Name = "created_at")]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "account")]
        public Account Account { get; private set; }

        [DataMember(Name = "status")]
        public Status Status { get; private set; }
    }
}
