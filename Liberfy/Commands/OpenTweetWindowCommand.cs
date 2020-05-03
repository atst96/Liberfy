using System.Linq;
using Liberfy.ViewModels;
using Livet.Messaging;
using WpfMvvmToolkit;

namespace Liberfy.Commands
{
    internal class OpenTweetWindowCommand : Command<IAccount>
    {
        private readonly MainWindowViewModel _viewModel;

        public OpenTweetWindowCommand(MainWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(IAccount parameter) => true;

        protected override void Execute(IAccount parameter)
        {
            var account = parameter ?? AccountManager.Accounts.FirstOrDefault();
            var viewModel = new TweetWindowViewModel();

            viewModel.SetPostAccount(account);

            this._viewModel.Messenger.Raise(new TransitionMessage(viewModel, "MsgKey_OpenTweetDialog"));
        }
    }
}
