using System.Linq;
using WpfMvvmToolkit;

namespace Liberfy.Commands
{
    internal class OpenTweetWindowCommand : Command<IAccount>
    {
        private ViewModels.MainWindowViewModel _viewModel;

        public OpenTweetWindowCommand(ViewModels.MainWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(IAccount parameter) => true;

        protected override void Execute(IAccount parameter)
        {
            var account = parameter ?? AccountManager.Accounts.FirstOrDefault();

            this._viewModel.WindowService.OpenTweetWindow(account);

        }
    }
}
