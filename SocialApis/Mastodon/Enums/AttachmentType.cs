using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public static class AttachmentTypes
    {
        public const string Image = "image";
        public const string Video = "video";
        public const string GifVideo = "gifv";
        public const string Unknown = "unknown";
        public const string Audio = "audio";
    }
}
