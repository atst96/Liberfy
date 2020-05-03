using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class AttachmentMeta
    {
        [DataMember(Name = "small")]
        public Meta Small { get; private set; }

        [DataMember(Name = "original")]
        public Meta Original { get; private set; }

        [DataMember(Name = "length")]
        [Utf8Json.JsonFormatter(typeof(Formatters.NullableValueFormatter<TimeSpan>))]
        public TimeSpan? Length { get; private set; }

        [DataMember(Name = "duration")]
        public double? Duration { get; private set; }

        [DataMember(Name = "fps")]
        public int? Fps { get; private set; }

        [DataMember(Name = "aspect")]
        public double? Aspect { get; private set; }

        [DataMember(Name = "audio_encode")]
        public string AudioEncode { get; private set; }

        [DataMember(Name = "audio_bitrate")]
        public string AudioBitrate { get; private set; }

        [DataMember(Name = "audio_channels")]
        public string AudioChannels { get; private set; }
    }
}
