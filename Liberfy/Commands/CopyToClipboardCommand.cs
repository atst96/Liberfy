﻿using WpfMvvmToolkit;

namespace Liberfy.Commands
{
    /// <summary>
    /// クリップボードにコピーするコマンド
    /// </summary>
    internal sealed class CopyToClipboardCommand : Command<string>
    {
        public CopyToClipboardCommand() : base(true)
        {
        }

        protected override bool CanExecute(string parameter)
        {
            return !string.IsNullOrEmpty(parameter);
        }

        protected override void Execute(string parameter)
        {
            System.Windows.Clipboard.SetText(parameter);
        }
    }
}
