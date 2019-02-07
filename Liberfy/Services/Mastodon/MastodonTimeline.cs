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
        public MastodonApi _tokens => _account.Tokens;

        public MastodonTimeline(MastodonAccount account)
            : base(account)
        {
            this._account = account;
            this._userId = account.Id;
        }

        public override async void Load()
        {
            Task[] tasks =
            {
                this.LoadHomeTimeline(),
                // this.LoadNotificationTimeline(),
                // this.LoadMessageTimeline(),
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public override void Unload()
        {
            this.AccountColumns.Clear();
        }

        private IEnumerable<StatusItem> GetStatusItem(IEnumerable<Status> statuses)
        {                                                            
            foreach (var status in statuses)
            {
                yield return new StatusItem(status, this._account);
            }
        }

        private async Task LoadHomeTimeline()
        {
            try
            {
                var statuses = await this._tokens.Timelines.Home().ConfigureAwait(false);
                var items = this.GetStatusItem(statuses);

                foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Home))
                {
                    _dispatcher.Invoke(() => column.Items.Reset(items));
                }
            }
            finally { }
        }
    }
}
