using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Model;
using SocialApis.Mastodon;

namespace Liberfy
{
    public static class MastodonValueConverter
    {
        private static readonly IReadOnlyDictionary<string, StatusVisibility> _stringStatusVisibilityMap = new Dictionary<string, StatusVisibility>
        {
            [StatusVisibilities.Public] = StatusVisibility.Public,
            [StatusVisibilities.Unlisted] = StatusVisibility.Unlisted,
            [StatusVisibilities.Direct] = StatusVisibility.Direct,
            [StatusVisibilities.Private] = StatusVisibility.Private,
        };

        public static StatusVisibility ToVisibility(string visibility)
        {
            return _stringStatusVisibilityMap[visibility];
        }

        private static readonly IReadOnlyDictionary<string, AttachmentType> _stringAttachmentTypeMap = new Dictionary<string, AttachmentType>
        {
            [AttachmentTypes.Unknown] = AttachmentType.Unknown,
            [AttachmentTypes.Image] = AttachmentType.Photo,
            [AttachmentTypes.GifVideo] = AttachmentType.Gif,
            [AttachmentTypes.Video] = AttachmentType.Video,
        };

        public static AttachmentType ToAttachmentType(string attachmentType)
        {
            return _stringAttachmentTypeMap[attachmentType];
        }
    }
}
