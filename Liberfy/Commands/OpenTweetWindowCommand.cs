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
            this._viewModel.WindowService.OpenTweetWindow(parameter);
        }
    }
}
