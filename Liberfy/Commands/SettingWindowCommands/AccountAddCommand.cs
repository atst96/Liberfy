using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModels;

namespace Liberfy.Commands.SettingWindowCommands
{
    internal class AccountAddCommand : Command
    {
        private SettingWindowViewModel _viewModel;

        public AccountAddCommand(SettingWindowViewModel viewModel) : base()
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter) => true;

        protected override void Execute(object parameter)
        {
            this._viewModel.WindowService.OpenAuthenticationWindow();
        }
    }
}
