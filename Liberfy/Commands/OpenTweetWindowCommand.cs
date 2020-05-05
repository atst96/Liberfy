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
            
        }
    }
}
