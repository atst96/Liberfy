using Liberfy.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    [Obsolete]
    /// <summary>
    /// 動的に生成可能なコマンド。
    /// </summary>
    /// <typeparam name="T">コマンドのパラメータの型</typeparam>
    internal class DelegateCommand<T> : Command<T>
    {
        private Action<T> _execute;
        private Predicate<T> _canExecute;

        /// <summary>
        /// <see cref="DelegateCommand{T}"/>クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        public DelegateCommand(Action<T> execute)
            : this(execute, DefaultCanExecute, false) { }

        /// <summary>
        /// <see cref="DelegateCommand{T}"/>クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        /// <param name="hookRequerySuggested">
        /// <seealso cref="CanExecuteChanged"/>へイベントが購読された際に、<seealso cref="CommandManager.RequerySuggested"/>への購読を行うかの設定
        /// </param>
        public DelegateCommand(Action<T> execute, bool hookRequerySuggested)
            : this(execute, DefaultCanExecute, hookRequerySuggested) { }

        /// <summary>
        /// <see cref="DelegateCommand{T}"/>クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを決定するメソッド</param>
        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
            : this(execute, canExecute, false) { }

        /// <summary>
        /// <see cref="DelegateCommand{T}"/>クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンドが起動する際に呼び出すメソッド</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを決定するメソッド</param>
        /// <param name="hookRequerySuggested">
        /// <seealso cref="CanExecuteChanged"/>へイベントが購読された際に、<seealso cref="CommandManager.RequerySuggested"/>への購読を行うかの設定
        /// </param>
        public DelegateCommand(Action<T> execute, Predicate<T> canExecute, bool hookRequerySuggested)
            : base(hookRequerySuggested)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        protected override bool CanExecute(T parameter) => this._canExecute.Invoke(parameter);

        protected override void Execute(T parameter) => this._execute.Invoke(parameter);

        public override void Dispose()
        {
            base.Dispose();

            this._execute = null;
            this._canExecute = null;
        }

        private static readonly Predicate<T> DefaultCanExecute = (_) => true;

        [Obsolete]
        public static CommandChain<T> When(Predicate<T> predicate) => new CommandChain<T>(predicate);

        [Obsolete]
        public static DelegateCommand<T> Execute(Action<T> action) => new DelegateCommand<T>(action);
    }
}
