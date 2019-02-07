using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Liberfy.Model;
using SocialApis.Twitter;

namespace Liberfy
{
    internal class TwitterDataStore : DataStoreBase<User, Status>, IDataStore
    {
        protected override UserInfo CreateAccountInfo(User account)
        {
            if (!account.Id.HasValue)
                throw new ArgumentException();

            var info = new UserInfo(ServiceType.Twitter, account.Id.Value)
            {
                CreatedAt = account.CreatedAt,
            };

            this.UpdateAccountInfo(info, account);

            return info;
        }

        protected override StatusInfo CreateStatusInfo(Status status)
        {
            if (status.RetweetedStatus != null)
                throw new ArgumentException();

            var info = new StatusInfo(ServiceType.Twitter)
            {
                Id = status.Id,
                CreatedAt = status.CreatedAt,
                FilterLevel = status.FilterLevel,
                InReplyToStatusId = status.InReplyToStatusId ?? -1,
                InReplyToUserId = status.InReplyToUserId ?? -1,

                IsQuotedStatus = status.IsQuotedStatus && status.QuotedStatus != null,

                Language = status.Language,
                PossiblySensitive = status.PossiblySensitive,
                Text = status.FullText ?? status.Text,
                User = this.RegisterAccount(status.User),

                Attachments = status.ExtendedEntities?.Media
                    .Select(m => new Attachment(m))
                    .ToArray() ?? new Attachment[0],
            };

            if (info.IsQuotedStatus)
            {
                info.QuotedStatus = this.RegisterStatus(status.QuotedStatus);
            }

            (info.SourceUrl, info.SourceName) = status.ParseSource();

            info.SetTextEntityBuilder(new TwitterTextTokenBuilder(info.Text ?? "", status.Entities));

            this.UpdateStatusInfo(info, status);

            return info;
        }

        protected override long GetAccountId(User account)
        {
            return account.Id ?? throw new ArgumentNullException("account.Id");
        }

        protected override long GetStatusId(Status status)
        {
            return status.Id;
        }

        protected override void UpdateAccountInfo(UserInfo info, User account)
        {
            const string RemoteUrlBase = "https://twitter.com/";

            info.LongUserName = account.ScreenName;
            info.Description = account.Description;

            info.SetDescriptionEntitiesBuilder(new TwitterTextTokenBuilder(account.Description ?? "", account.Entities?.Description));
            info.SetUrlEntitiesBuilder(new TwitterTextTokenBuilder(account.Url ?? "", account.Entities?.Url));

            info.Url = account.Url;

            info.FollowersCount = account.FollowersCount;
            info.FriendsCount = account.FriendsCount;
            info.Language = account.Language;
            info.Location = account.Location;
            info.Name = account.Name;
            info.ProfileBannerUrl = account.ProfileBannerUrl;
            info.ProfileImageUrl = account.ProfileImageUrl;
            info.IsProtected = account.IsProtected;
            info.ScreenName = account.ScreenName;
            info.StatusesCount = account.StatusesCount;
            info.RemoteUrl = RemoteUrlBase + account.ScreenName;

            info.UpdatedAt = DateTime.Now;
        }

        protected override void UpdateStatusInfo(StatusInfo info, Status status)
        {
            if ((status.RetweetedStatus ?? status).Id != info.Id)
                throw new ArgumentException();

            info.FavoriteCount = status.FavoriteCount ?? info.FavoriteCount;
            info.RetweetCount = status.RetweetCount ?? info.RetweetCount;
        }
    }
}
