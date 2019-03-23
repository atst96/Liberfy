using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DirectMessage
    {
        [DataMember(Name = "created_at")]
        [Utf8Json.JsonFormatter(typeof(TwitterTimeFormatFormatter))]
        public DateTimeOffset CreatedAt { get; set; }

        [DataMember(Name = "entities")]
        public Entities Entities { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "recipient")]
        public User Recipient { get; set; }

        [DataMember(Name = "recipient_id")]
        public long RecipientId { get; set; }

        [DataMember(Name = "recipient_screen_name")]
        public string RecipientScreenName { get; set; }

        [DataMember(Name = "sender")]
        public User Sender { get; set; }

        [DataMember(Name = "sender_id")]
        public long SenderId { get; set; }

        [DataMember(Name = "sender_screen_name")]
        public string SenderScreenName { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }
    }
}
