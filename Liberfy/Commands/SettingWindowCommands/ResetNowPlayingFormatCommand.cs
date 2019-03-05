using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands.SettingWindowCommands
{
    internal class ResetNowPlayingFormatCommand : Command
    {
        private readonly SettingWindowViewModel _viewModel;

        public ResetNowPlayingFormatCommand(SettingWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter) => true;

        protected override void Execute(object parameter)
        {
            this._viewModel.NowPlayingFormat = Defaults.DefaultNowPlayingFormat;
        }
    }
}
