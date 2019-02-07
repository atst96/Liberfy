using Liberfy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands
{
    internal class RemoveMediaCommand : Command<UploadMedia>
    {
        private TweetWindow _viewModel;

        public RemoveMediaCommand(TweetWindow viewModel) : base(true)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(UploadMedia parameter)
        {
            Console.WriteLine(parameter);

            bool value = this._viewModel.Media.Contains(parameter);

            Console.WriteLine("RemoveMediaCommand-CanExecute: " + value);

            return this._viewModel.Media.Contains(parameter);
        }

        protected override void Execute(UploadMedia parameter)
        {
            using (parameter)
            {
                this._viewModel.Media.Remove(parameter);
                this._viewModel.UpdateCanPost();
            }
        }
    }
}
