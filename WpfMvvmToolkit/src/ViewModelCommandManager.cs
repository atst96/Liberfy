using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WpfMvvmToolkit
{
    internal class ViewModelCommandManager
    {
        /// <summary>
        /// 生成コマンド情報
        /// </summary>
        private List<IDisposableCommand> _commands = new();

        /// <summary>
        /// 内部リストにコマンドを登録する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newCommand"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Add<T>(T newCommand) where T : IDisposableCommand
        {
            if (this._isDisposed)
            {
                throw new InvalidOperationException("This instance was already disposed.");
            }

            this._commands.Add(newCommand);
            return newCommand;
        }

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Command Create(Action action)
            => this.Add(new DelegateCommand(action));

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="hookRequerySuggested">RequerySuggestedイベントにフックするかどうかのフラグ</param>
        /// <returns></returns>
        public Command Create(Action action, bool hookRequerySuggested)
            => this.Add(new DelegateCommand(action, hookRequerySuggested));

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public Command Create(Action action, Func<bool> canExecute)
            => this.Add(new DelegateCommand(action, canExecute));

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        /// <param name="hookRequerySuggested">RequerySuggestedイベントにフックするかどうかのフラグ</param>
        /// <returns></returns>
        public Command Create(Action action, Func<bool> canExecute, bool hookRequerySuggested)
            => this.Add(new DelegateCommand(action, canExecute, hookRequerySuggested));

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public Command<T> Create<T>(Action<T> action)
            => this.Add(new DelegateCommand<T>(action));

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="hookRequerySuggested">RequerySuggestedイベントにフックするかどうかのフラグ</param>
        /// <returns></returns>
        public Command<T> Create<T>(Action<T> action, bool hookRequerySuggested)
            => this.Add(new DelegateCommand<T>(action, hookRequerySuggested));

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public Command<T> Create<T>(Action<T> action, Predicate<T> canExecute)
            => this.Add(new DelegateCommand<T>(action, canExecute));

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        /// <param name="hookRequerySuggested">RequerySuggestedイベントにフックするかどうかのフラグ</param>
        /// <returns></returns>
        public Command<T> Create<T>(Action<T> action, Predicate<T> canExecute, bool hookRequerySuggested)
            => this.Add(new DelegateCommand<T>(action, canExecute, hookRequerySuggested));

        /// <summary>
        /// 破棄済みフラグ
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// インスタンスを破棄する
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._commands.ForEach(c => c.Dispose());
                    this._commands.Clear();
                }

                this._isDisposed = true;
                this._commands = null;
            }
        }

        /// <summary>
        /// インスタンスを破棄する
        /// </summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
