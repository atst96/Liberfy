using Liberfy.Settings;
using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class TwitterAccount : AccountBase, IAccount<Tokens>
    {
        public override long Id { get; protected set; }

        public override SocialService Service { get; } = SocialService.Twitter;

        protected override DataStore DataStore { get; } = DataStore.Twitter;

        public Tokens InternalTokens { get; private set; }

        public override ITokensBase Tokens => this.InternalTokens;

        public TwitterAccount(AccountItem accountItem)
            : base(accountItem)
        {
        }

        public TwitterAccount(Tokens tokens, User account)
            : base(tokens, account)
        {
        }

        public override void SetTokens(ApiTokenInfo t)
        {
            this.InternalTokens = new Tokens(
                t.ConsumerKey,
                t.ConsumerSecret,
                t.AccessToken,
                t.AccessTokenSecret);
        }

        protected override TimelineBase CreateTimeline()
        {
            return new TwitterTimeline(this);
        }

        public override async Task Load()
        {
            if (await base.TryLogin())
            {
                await this.GetDetails();
                this.StartTimeline();
            }
        }

        protected override async Task<bool> Login()
        {
            try
            {
                var user = await this.InternalTokens.Account.VerifyCredentials(new Query
                {
                    ["include_entities"] = true,
                    ["skip_status"] = true,
                    ["include_email"] = false
                });

                this.Id = user.Id.Value;

                this.Info.Update(user);

                return true;
            }
            catch (TwitterException tex)
            {
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
            var tasks = new[]
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
            return this.GetIdsAndSetList(this.InternalTokens.Friends.Ids, this.FollowingIds, "フォロー中一覧");
        }

        private Task GetFollowerIds()
        {
            return this.GetIdsAndSetList(this.InternalTokens.Followers.Ids, this.FollowersIds, "フォロワー一覧");
        }

        private Task GetBlockedIds()
        {
            return Setting.GetBlockedIdsAtLoadingAccount
                ? this.GetIdsAndSetList(this.InternalTokens.Blocks.Ids, this.BlockedIds, "ブロック中一覧")
                : Task.CompletedTask;
        }

        private Task GetMutedIds()
        {
            return Setting.GetMutedIdsAtLoadingAccount
                ? this.GetIdsAndSetList(this.InternalTokens.Mutes.Ids, this.MutedIds, "ミュート中一覧")
                : Task.CompletedTask;
        }

        private Task GetIncomingIds()
        {
            return this.Info.IsProtected
                ? this.GetIdsAndSetList(this.InternalTokens.Friendships.Incoming, this.IncomingIds, "フォロー申請一覧")
                : Task.CompletedTask;
        }

        private Task GetOutgoingIds()
        {
            return this.GetIdsAndSetList(this.InternalTokens.Friendships.Outgoing, this.OutgoingIds, "フォロー申請中一覧");
        }

        private async Task GetIdsAndSetList(Func<int, Task<CursoredIdsResponse>> apiCallFunc, HashSet<long> hashSet, string dataLabel)
        {
            try
            {
                int? cursor = null;

                do
                {
                    var ids = await apiCallFunc(cursor ?? -1);
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

        protected override IAccountCommandGroup CreateCommands()
        {
            return new Twitter.AccountCommandGroup(this);
        }

        #endregion GetDetails
    }
}
