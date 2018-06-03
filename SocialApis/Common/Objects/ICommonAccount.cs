using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Common
{
    public interface ICommonAccount
    {
        SocialService Service { get; }

        long? Id { get; }

        DateTimeOffset CreatedAt { get; }

        string UserName { get; }

        string DisplayName { get; }

        string LongUserName { get; }

        string Description { get; }

        string Location { get; }

        bool IsProtected { get; }

        int FollowersCount { get; }

        int FollowingCount { get; }

        int StatusesCount { get; }

        string Language { get; }

        string AvatarImageUrl { get; }

        string HeaderImageUrl { get; }

        bool? IsSuspended { get; }

        string Url { get; }

        string RemoteUrl { get; }

        EntityBase[] DescriptionEntities { get; }
        
        EntityBase[] UrlEntities { get; }
    }
}
