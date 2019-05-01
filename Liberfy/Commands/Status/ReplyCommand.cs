using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Components;
using Liberfy.ViewModels;
using WpfMvvmToolkit;

namespace Liberfy.Commands.Status
{
    internal class ReplyCommand : Command
    {
        public readonly StatusItem _item;

        public ReplyCommand(StatusItem item)
        {
            this._item = item;
        }

        protected override bool CanExecute(object parameter)
        {
            return this._item != null;
        }

        protected override void Execute(object parameter)
        {
            var mainView = App.Instance.FindViewModel<MainWindowViewModel>().FirstOrDefault();

            mainView.WindowService.OpenTweetWindow(this._item);
        }
    }
}
