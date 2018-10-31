using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Twitter.Commands
{
    internal class RepostCommand : Command<StatusItem>
    {
        public RepostCommand() : base(true) { }

        protected override bool CanExecute(StatusItem parameter)
        {
            return parameter?.CanRetweet ?? false;
        }

        protected override async void Execute(StatusItem item)
        {
            if (item.Account is TwitterAccount account)
            {
                var tokens = account.InternalTokens;

                StatusResponse status = default;

                try
                {
                    status = item.Reaction.IsRetweeted
                        ? await tokens.Statuses.Unretweet(item.Status.Id)
                        : await tokens.Statuses.Retweet(item.Status.Id);

                    DataStore.Twitter.StatusAddOrUpdate(status);

                    item.Reaction.IsRetweeted = status.IsRetweeted ?? !item.Reaction.IsRetweeted;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
