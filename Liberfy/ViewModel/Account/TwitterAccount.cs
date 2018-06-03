using Liberfy.Settings;
using Liberfy.ViewModel.Timeline;
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
        protected override DataStore DataStore => DataStore.Twitter;

        public Tokens InternalTokens { get; private set; }

        public override ITokensBase Tokens => this.InternalTokens;

        public override long Id { get; protected set; }

        public override SocialService Service { get; } = SocialService.Twitter;

        public TwitterAccount(AccountItem accountItem) 
            : base(accountItem)
        {
        }

        public TwitterAccount(Tokens tokens, IEnumerable<ColumnOptionBase> columnOptions = null)
            : this(tokens, null, columnOptions)
        {
        }

        public TwitterAccount(Tokens tokens, User account, IEnumerable<ColumnOptionBase> columnOptions = null)
            : base(tokens, account, columnOptions)
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

        protected override ITimeline CreateTimeline(IEnumerable<ColumnOptionBase> columnOptions)
        {
            return new TwitterTimeline(this, columnOptions);
        }

        protected override async Task<bool> Login()
        {
            try
            {
                var user = await this.InternalTokens.Account.VerifyCredentials(new SocialApis.Query
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

        protected override Task LoadDetails()
        {
            return Task.WhenAll(
                this.LoadFollowerIds(),
                this.LoadFollowingIds(),
                this.LoadBlockedIds(),
                this.LoadMutedIds(),
                this.LoadOutgoingIds(),
                this.LoadIncomingIds()
            );
        }

        #region LoadDetailsメソッド郡

        private Task LoadFollowingIds()
            => this.GetIdsList(this.InternalTokens.Friends.Ids, this.FollowingIds, "フォロー中一覧");

        private Task LoadFollowerIds()
            => this.GetIdsList(this.InternalTokens.Followers.Ids, this.FollowersIds, "フォロワー一覧");

        private Task LoadBlockedIds()
        {
            return Setting.GetBlockedIdsAtLoadingAccount
                ? this.GetIdsList(this.InternalTokens.Blocks.Ids, this.BlockedIds, "ブロック中一覧")
                : Task.CompletedTask;
        }

        private Task LoadMutedIds()
        {
            return Setting.GetMutedIdsAtLoadingAccount
                ? this.GetIdsList(this.InternalTokens.Mutes.Ids, this.MutedIds, "ミュート中一覧")
                : Task.CompletedTask;
        }

        private Task LoadIncomingIds()
        {
            return this.Info.IsProtected
                ? this.GetIdsList(this.InternalTokens.Friendships.Incoming, this.IncomingIds, "フォロー申請一覧")
                : Task.CompletedTask;
        }

        private Task LoadOutgoingIds()
            => this.GetIdsList(this.InternalTokens.Friendships.Outgoing, this.OutgoingIds, "フォロー申請中一覧");

        private async Task GetIdsList(Func<int, Task<CursoredIdsResponse>> apiCallFunc, HashSet<long> hashSet, string dataLabel)
        {
            try
            {
                int cursor = -1;

                do
                {
                    var ids = await apiCallFunc(cursor);
                    cursor = ids.NextCursor ?? 0;

                    hashSet.UnionWith(ids.Ids);
                }
                while (cursor != 0);
            }
            catch (Exception ex)
            {
                this.SetErrorMessage(dataLabel, ex.Message);
            }
        }

        #endregion LoadDetails
    }
}
