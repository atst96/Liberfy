using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Liberfy.Model;
using SocialApis.Twitter;

namespace Liberfy.Services.Twitter
{
    internal class TwitterDataStore : DataStoreBase<User, Status>, IDataStore
    {
        public override UserInfo RegisterAccount(User account)
        {
            long id = account.Id ?? throw new ArgumentException(nameof(account));

            UserInfo info;

            if (this.Accounts.TryGetValue(id, out info))
            {
                this.UpdateAccountInfo(info, account);
            }
            else
            {
                info = this.CreateAccountInfo(account);
                this.Accounts.TryAdd(id, info);
            }

            return info;
        }

        private UserInfo CreateAccountInfo(User account)
        {
            long id = account.Id ?? throw new ArgumentException(nameof(account));

            var info = new UserInfo(ServiceType.Twitter, null, account.Id.Value)
            {
                CreatedAt = account.CreatedAt,
            };

            this.UpdateAccountInfo(info, account);

            return info;
        }

        private void UpdateAccountInfo(UserInfo info, User account)
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

        public override StatusInfo RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(nameof(status));

            StatusInfo info;

            if (this.Statuses.TryGetValue(id, out info))
            {
                this.UpdateStatusInfo(info, status);
            }
            else
            {
                info = this.CreateStatusInfo(status);
                this.Statuses.TryAdd(id, info);
            }

            return info;
        }

        private StatusInfo CreateStatusInfo(Status status)
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
                Language = status.Language,
                PossiblySensitive = status.PossiblySensitive,
                Text = status.FullText ?? status.Text,
                User = this.RegisterAccount(status.User),
                Attachments = GetAttachments(status.ExtendedEntities?.Media)
            };

            if (status.QuotedStatus != null)
            {
                info.QuotedStatus = this.RegisterStatus(status.QuotedStatus);
            }

            (info.SourceUrl, info.SourceName) = ExpandClientInfo(status);

            info.SetTextEntityBuilder(new TwitterTextTokenBuilder(info.Text ?? "", status.Entities));

            this.UpdateStatusInfo(info, status);

            return info;
        }

        private static Attachment[] GetAttachments(SocialApis.Twitter.MediaEntity[] entities)
        {
            var results = new Attachment[entities?.Length ?? 0];

            for (int idx = 0; idx < results.Length; ++idx)
            {
                results[idx] = new Attachment(entities[idx]);
            }

            return results;
        }

        private static (string url, string sourceName) ExpandClientInfo(Status status)
        {
            var match = Regexes.TwitterSourceHtml.Match(status.Source);

            return match?.Success ?? false
                ? (string.Empty, status.Source)
                : (match.Groups["url"].Value, match.Groups["name"].Value);
        }

        private void UpdateStatusInfo(StatusInfo info, Status status)
        {
            if ((status.RetweetedStatus ?? status).Id != info.Id)
            {
                throw new ArgumentException();
            }

            info.FavoriteCount = status.FavoriteCount ?? info.FavoriteCount;
            info.RetweetCount = status.RetweetCount ?? info.RetweetCount;
        }
    }
}
