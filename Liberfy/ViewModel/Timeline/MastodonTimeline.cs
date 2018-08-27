using SocialApis.Mastodon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
    internal class MastodonTimeline : TimelineBase
    {
        private readonly static Dispatcher _dispatcher = App.Current.Dispatcher;

        private readonly long _userId;
        private readonly MastodonAccount _account;
        public Tokens _tokens => _account.InternalTokens;

        public MastodonTimeline(MastodonAccount account)
        {
            this._account = account;
            this._userId = account.Id;
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }
    }
}
