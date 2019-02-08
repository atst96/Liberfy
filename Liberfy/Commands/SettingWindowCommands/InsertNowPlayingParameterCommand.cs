using Liberfy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands.SettingWindowCommands
{
    internal class InsertNowPlayingParameterCommand : Command<string>
    {
        private readonly SettingWindowViewModel _viewModel;

        public InsertNowPlayingParameterCommand(SettingWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(string parameter) => !string.IsNullOrWhiteSpace(parameter);

        protected override void Execute(string parameter)
        {
            this._viewModel.NowPlayingTextBoxController.Insert(parameter);
            this._viewModel.NowPlayingTextBoxController.Focus();
        }
    }
}
