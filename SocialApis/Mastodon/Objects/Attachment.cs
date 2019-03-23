using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Attachment
    {
        [DataMember(Name = "id")]
        [Utf8Json.JsonFormatter(typeof(Formatters.StringToLongFormatter))]
        public long Id { get; private set; }
        
        [DataMember(Name = "type")]
        public AttachmentType Type { get; private set; }

        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "remote_url")]
        public string RemoteUrl { get; private set; }

        [DataMember(Name = "preview_url")]
        public string PreviewUrl { get; private set; }

        [DataMember(Name = "text_uri")]
        public string TextUri { get; private set; }

        [DataMember(Name = "meta")]
        public AttachmentMeta Meta { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }
    }
}
