using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModels;
using WpfMvvmToolkit;

namespace Liberfy.Commands.SettingWindowCommands
{
    internal class ColumnAddCommand : Command<ColumnType>
    {
        private readonly SettingWindowViewModel _viewModel;

        public ColumnAddCommand(SettingWindowViewModel viewModel) : base()
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(ColumnType parameter) => true;

        protected override void Execute(ColumnType parameter)
        {
            this._viewModel.DefaultColumns.Add(ColumnBase.FromType(parameter));
        }
    }
}
