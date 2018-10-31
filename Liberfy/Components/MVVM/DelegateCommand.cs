﻿using Liberfy.Component;
using System;

namespace Liberfy
{
    [Obsolete]
    /// <summary>
    /// 動的に生成可能なコマンド。
    /// </summary>
    internal class DelegateCommand : Command
    {
        private Action _execute;
        private Func<bool> _canExecute;

        /// <summary>
        /// <see cref="DelegateCommand" />クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        public DelegateCommand(Action execute)
            : this(execute, DefaultCanExecute, false) { }

        /// <summary>
        /// <see cref="DelegateCommand" />クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        /// <param name="hookRequerySuggested">
        /// <seealso cref="CanExecuteChanged"/>へイベントが購読された際に、<seealso cref="CommandManager.RequerySuggested"/>への購読を行うかの設定
        /// </param>
        public DelegateCommand(Action execute, bool hookRequerySuggested)
            : this(execute, DefaultCanExecute, hookRequerySuggested) { }

        /// <summary>
        /// <see cref="DelegateCommand" />クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを決定するメソッド</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
            : this(execute, canExecute, false) { }

        /// <summary>
        /// <see cref="DelegateCommand" />クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを決定するメソッド</param>
        /// <param name="hookRequerySuggested">
        /// <seealso cref="CanExecuteChanged"/>へイベントが購読された際に、<seealso cref="CommandManager.RequerySuggested"/>への購読を行うかの設定
        /// </param>
        public DelegateCommand(Action execute, Func<bool> canExecute, bool hookRequerySuggested)
            : base(hookRequerySuggested)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        protected override bool CanExecute(object parameter) => this._canExecute.Invoke();

        protected override void Execute(object parameter) => this._execute.Invoke();

        public override void Dispose()
        {
            base.Dispose();

            this._execute = null;
            this._canExecute = null;
        }

        private static readonly Func<bool> DefaultCanExecute = () => true;

        [Obsolete]
        public static CommandChain When(Func<bool> action) => new CommandChain(action);

        [Obsolete]
        public static DelegateCommand Execute(Action action) => new DelegateCommand(action);
    }
}
