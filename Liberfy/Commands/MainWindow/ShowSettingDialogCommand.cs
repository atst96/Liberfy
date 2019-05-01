using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModels;
using WpfMvvmToolkit;

namespace Liberfy.Commands.MainWindow
{
    internal class ShowSettingDialogCommand : Command
    {
        private readonly MainWindowViewModel _viewModel;

        public ShowSettingDialogCommand(MainWindowViewModel viewModel) : base()
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter) => true;

        protected override void Execute(object parameter)
        {
            this._viewModel.WindowService.OpenSetting();
        }
    }
}
