using System;
using Liberfy.Commands.Status;
using WpfMvvmToolkit;

namespace Liberfy.Commands
{
    internal class StatusCommandGroup
    {
        private readonly StatusItem item;

        public Command ReplyCommand { get; }
        public Command RetweetCommand { get; }
        public Command FavoriteCommand { get; }
        public Command ShowDetailsCommand { get; }

        public StatusCommandGroup(StatusItem item)
        {
            this.item = item ?? throw new ArgumentNullException(nameof(item));

            this.ReplyCommand = new ReplyCommand(item);
            this.FavoriteCommand = new FavoriteCommand(item);
            this.RetweetCommand = new RetweetCommand(item);
            // this.ShowDetailsCommand = new ShowDetailsCommand(item);
        }
    }
}
