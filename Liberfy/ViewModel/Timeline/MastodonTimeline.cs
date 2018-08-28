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
            this.LoadHomeTimeline();
            // this.LoadNotificationTimeline();
            // this.LoadMessageTimeline();
        }

        public override void Unload()
        {
        }

        private IEnumerable<StatusItem> GetStatusItem(IEnumerable<Status> statuses)
        {
            foreach (var status in statuses)
            {
                yield return new StatusItem(status, this._account);
            }
        }

        private IEnumerable<ColumnBase> GetCurrentAccountColumns()
        {
            foreach (var column in TimelineBase.Columns)
            {
                if (column.Account?.Equals(this._account) ?? false)
                    yield return column;
            }
        }

        private async Task LoadHomeTimeline()
        {
            try
            {
                var statuses = await this._tokens.Timelines.Home();
                var items = this.GetStatusItem(statuses);

                foreach (var column in this.GetCurrentAccountColumns().Where(c => c.Type == ColumnType.Home))
                {
                    column.Items.Reset(items);
                }
            }
            finally { }
        }
    }
}
