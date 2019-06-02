using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Liberfy.Model;
using SocialApis;
using SocialApis.Common;

namespace Liberfy
{
    internal interface IUserInfo<T> : IUserInfo
    {
        IUserInfo<T> Update(T user);
    }

    internal interface IUserInfo : IEquatable<IUserInfo>, INotifyPropertyChanged
    {
        ServiceType Service { get; }
        long Id { get; }
        Uri Host { get; }
        DateTimeOffset CreatedAt { get; }
        string LongUserName { get; }
        string Description { get; }
        IEnumerable<IEntity> DescriptionEntities { get; }
        IEnumerable<IEntity> UrlEntities { get; }
        int FollowersCount { get; }
        int FollowingsCount { get; }
        string Language { get; }
        string Location { get; }
        string Name { get; }
        string ProfileBannerUrl { get; }
        string ProfileImageUrl { get; }
        bool IsProtected { get; }
        string ScreenName { get; }
        int StatusesCount { get; }
        bool IsSuspended { get; }
        string Url { get; }
        string RemoteUrl { get; }
        DateTime UpdatedAt { get; }
    }
}
