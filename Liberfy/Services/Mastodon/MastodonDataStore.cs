using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Liberfy.Model;
using SocialApis.Mastodon;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonDataStore : DataStoreBase<Account, Status>, IDataStore
    {
        private readonly Uri _host;

        public MastodonDataStore(Uri host)
        {
            this._host = host;
        }

        public override UserInfo RegisterAccount(Account account)
        {
            long id = account?.Id ?? throw new ArgumentException(nameof(account));

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

        private UserInfo CreateAccountInfo(Account account)
        {
            var info = new UserInfo(ServiceType.Mastodon, this._host, account.Id)
            {
                CreatedAt = account.CreatedAt,
            };

            this.UpdateAccountInfo(info, account);

            return info;
        }

        private void UpdateAccountInfo(UserInfo info, Account account)
        {
            info.LongUserName = account.Acct;
            info.Description = account.Note;

            info.Url = account.Url;

            info.UrlEntities = new[] { new PlainTextEntity(account.Url) };

            info.FollowersCount = account.FollowersCount;
            info.FriendsCount = account.FollowersCount;
            //info.Language = item.Language;
            //info.Location = item.Location;
            info.Name = account.DisplayName;
            info.ProfileBannerUrl = account.Header;
            info.ProfileImageUrl = account.Avatar;
            info.IsProtected = account.IsLocked;
            info.ScreenName = account.UserName;
            info.StatusesCount = account.StatusesCount;
            info.RemoteUrl = account.Url;

            info.UpdatedAt = DateTime.Now;
        }

        public override StatusInfo RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(nameof(status));

            StatusInfo info;

            if (this.Statuses.TryGetValue(id, out info))
            {
                if (!App.Status.IsAccountLoaded)
                {
                    this.UpdateStatusInfo(info, status);
                }
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
            if (status.Reblog != null)
            {
                throw new ArgumentException(nameof(status));
            }

            var info = new StatusInfo(ServiceType.Mastodon)
            {
                Id = status.Id,
                CreatedAt = status.CreatedAt,
                InReplyToStatusId = status.InReplyToId ?? -1,
                InReplyToUserId = status.InReplyToAccountId ?? -1,
                Language = status.Language,
                PossiblySensitive = status.Sensitive,
                SpoilerText = status.SpoilerText,
                Text = status.Content,
                User = this.RegisterAccount(status.Account),
                Attachments = status.MediaAttachments
                    .Select(m => new Model.Attachment(m))
                    .ToArray() ?? new Model.Attachment[0],
            };

            if (status.Application != null)
            {
                info.SourceName = status.Application.Name;
                info.SourceUrl = status.Application.Website;
            }

            info.SetTextEntityBuilder(new MastodonTextEntityBuilder(info.Text ?? "", status.Emojis));

            this.UpdateStatusInfo(info, status);

            return info;
        }

        private void UpdateStatusInfo(StatusInfo info, Status status)
        {
            if ((status.Reblog ?? status).Id != info.Id)
                throw new ArgumentException();

            info.FavoriteCount = status.FavouritesCount;
            info.RetweetCount = status.ReblogsCount;
        }
    }
}
