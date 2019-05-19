using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Model;
using SocialApis.Twitter;

namespace Liberfy
{
    internal static class TwitterValueConverter
    {
        private static readonly IReadOnlyDictionary<string, AttachmentType> _stringMediaTypeMap = new Dictionary<string, AttachmentType>
        {
            [MediaTypes.Photo] = AttachmentType.Photo,
            [MediaTypes.AnimatedGif] = AttachmentType.Gif,
            [MediaTypes.Video] = AttachmentType.Video,
        };

        public static AttachmentType ToAttachmentType(string attachmentType)
        {
            return _stringMediaTypeMap[attachmentType];
        }
    }
}
