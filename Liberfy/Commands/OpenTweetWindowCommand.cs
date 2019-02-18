namespace Liberfy.Commands
{
    internal class OpenTweetWindowCommand : Command<IAccount>
    {
        private ViewModel.MainWindow _viewModel;

        public OpenTweetWindowCommand(ViewModel.MainWindow viewModel)
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
