using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Twitter.Commands
{
    internal class LikeCommand : Command<StatusItem>
    {
        public LikeCommand() : base(true) { }

        protected override bool CanExecute(StatusItem item)
        {
            return item != null;
        }

        protected override async void Execute(StatusItem item)
        {
            if (item.Account is TwitterAccount account)
            {
                var tokens = account.Tokens;

                StatusResponse status = default;

                try
                {
                    status = item.Reaction.IsFavorited
                        ? await tokens.Favorites.Destroy(item.Status.Id)
                        : await tokens.Favorites.Create(item.Status.Id);

                    DataStore.Twitter.RegisterStatus(status);

                    item.Reaction.IsFavorited = status.IsFavorited ?? !item.Reaction.IsFavorited;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
