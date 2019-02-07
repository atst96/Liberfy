using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class AccountLoader
    {
        public async void Run(ICollection<IAccount> accounts)
        {
            var loadingTasks = new LinkedList<Task>();

            foreach (var account in accounts)
            {
                var task = account.Load();
                loadingTasks.AddLast(task);
            }

            await Task.WhenAll(loadingTasks.ToArray());

            App.Status.IsAccountLoaded = true;
        }
    }
}
