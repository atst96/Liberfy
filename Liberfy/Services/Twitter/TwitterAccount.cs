using Liberfy.Services;
using Liberfy.Services.Common;
using Liberfy.Services.Twitter;
using Liberfy.Settings;
using SocialApis;
using SocialApis.Common;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class TwitterAccount : AccountBase<TwitterApi, TwitterTimeline, User, Status>
    {
        public static readonly Uri ServerHostUrl = new Uri("https://twitter.com/", UriKind.Absolute);

        public override long Id { get; protected set; }

        public override ServiceType Service { get; } = ServiceType.Twitter;

        public TwitterAccount(AccountItem accountItem)
            : base(ServerHostUrl, accountItem)
        {
        }

        public TwitterAccount(TwitterApi tokens, User account)
            : base((long)account.Id, ServerHostUrl, account, tokens)
        {
        }

        private IAccountCommandGroup _commands;
        public override IAccountCommandGroup Commands => _commands ?? (_commands = new Twitter.AccountCommandGroup(this));

        public override DataStoreBase<User, Status> DataStore { get; } = global::Liberfy.DataStore.Twitter;

        public override IValidator Validator { get; } = new TwitterValidator();

        public override IServiceConfiguration ServiceConfiguration { get; } = new TwitterServiceConfiguration();

        private IApiGateway _apiGateway;
        public override IApiGateway ApiGateway => this._apiGateway ?? (this._apiGateway = new TwitterApiGateway(this.Tokens));

        protected override TwitterApi TokensFromApiTokenInfo(ApiTokenInfo tokens)
        {
            return new TwitterApi(
                tokens.ConsumerKey,
                tokens.ConsumerSecret,
                tokens.AccessToken,
                tokens.AccessTokenSecret
            );
        }

        protected override TwitterTimeline CreateTimeline() => new TwitterTimeline(this);

        public override async Task Load()
        {
            if (await base.Login().ConfigureAwait(false))
            {
                await this.GetDetails().ConfigureAwait(false);
                this.StartTimeline();
            }
        }

        protected override async Task<bool> VerifyCredentials()
        {
            try
            {
                var user = await this.Tokens.Account.VerifyCredentials(new Query
                {
                    ["include_entities"] = true,
                    ["skip_status"] = true,
                    ["include_email"] = false
                });

                this.Id = user.Id.Value;

                this.DataStore.RegisterAccount(user);

                return true;
            }
            catch (TwitterException tex)
            {
                Console.WriteLine(tex.Message);

                if (tex.InnerException is WebException wex)
                {
                    switch (wex.Status)
                    {
                        case WebExceptionStatus.Success:
                            break;
                    }
                }
            }

            return false;
        }

        protected override Task GetDetails()
        {
            Task[] tasks =
            {
                this.GetFollowingIds(),
                this.GetFollowerIds(),
                this.GetBlockedIds(),
                this.GetMutedIds(),
                this.GetOutgoingIds(),
                this.GetIncomingIds()
            };

            return Task.WhenAll(tasks);
        }

        #region GetDetailsメソッド郡

        private Task GetFollowingIds()
        {
            return this.GetIdsAndSetList(this.Tokens.Friends.Ids, this.FollowingIds, "フォロー中一覧");
        }

        private Task GetFollowerIds()
        {
            return this.GetIdsAndSetList(this.Tokens.Followers.Ids, this.FollowersIds, "フォロワー一覧");
        }

        private Task GetBlockedIds()
        {
            return Setting.GetBlockedIdsAtLoadingAccount
                ? this.GetIdsAndSetList(this.Tokens.Blocks.Ids, this.BlockedIds, "ブロック中一覧")
                : Task.CompletedTask;
        }

        private Task GetMutedIds()
        {
            return Setting.GetMutedIdsAtLoadingAccount
                ? this.GetIdsAndSetList(this.Tokens.Mutes.Ids, this.MutedIds, "ミュート中一覧")
                : Task.CompletedTask;
        }

        private Task GetIncomingIds()
        {
            return this.Info.IsProtected
                ? this.GetIdsAndSetList(this.Tokens.Friendships.Incoming, this.IncomingIds, "フォロー申請一覧")
                : Task.CompletedTask;
        }

        private Task GetOutgoingIds()
        {
            return this.GetIdsAndSetList(this.Tokens.Friendships.Outgoing, this.OutgoingIds, "フォロー申請中一覧");
        }

        private async Task GetIdsAndSetList(Func<int, Task<CursoredIdsResponse>> apiCallFunc, HashSet<long> hashSet, string dataLabel)
        {
            try
            {
                int? cursor = null;

                do
                {
                    var ids = await apiCallFunc(cursor ?? -1).ConfigureAwait(false);
                    cursor = ids.NextCursor;

                    hashSet.UnionWith(ids.Ids);
                }
                while (cursor.HasValue && cursor.Value > 0);
            }
            catch (Exception ex)
            {
                this.SetErrorMessage(dataLabel, ex.Message);
            }
        }

        protected override UserInfo GetUserInfo(User account)
        {
            return this.DataStore.RegisterAccount(account);
        }

        #endregion GetDetails
    }
}
