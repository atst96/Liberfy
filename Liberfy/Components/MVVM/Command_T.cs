using System;
using System.Windows.Input;

namespace Liberfy
{
    /// <summary>
    /// コマンドを定義する際に基となるクラス。
    /// </summary>
    /// <typeparam name="T">コマンドのパラメータの型</typeparam>
    internal abstract class Command<T> : ICommand, IDisposable
    {
        private readonly bool hookRequerySuggested;
        private EventHandler dummyCanExecuteChanged;
        private readonly WeakCollection<EventHandler> _events = new WeakCollection<EventHandler>();

        /// <summary>
        /// <see cref="Command{T}"/>クラスのインスタンスを生成します。
        /// </summary>
        protected Command()
        {
        }

        /// <summary>
        /// <see cref="Command{T}"/>クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="hookRequerySuggested">
        /// <seealso cref="CanExecuteChanged"/>へイベントが購読された際に、<see cref="CommandManager"/>.<seealso cref="RequerySuggested"/>への購読も行うかの設定
        /// </param>
        protected Command(bool hookRequerySuggested)
        {
            this.hookRequerySuggested = hookRequerySuggested;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                this.dummyCanExecuteChanged += value;
                if (this.hookRequerySuggested)
                {
                    CommandManager.RequerySuggested += value;
                }
                this._events.Add(value);
            }
            remove
            {
                this.dummyCanExecuteChanged -= value;
                if (this.hookRequerySuggested)
                {
                    CommandManager.RequerySuggested -= value;
                }
                this._events.Remove(value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter is T _p ? _p : default);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter is T _p ? _p : default);
        }

        /// <summary>
        /// <seealso cref="ICommand.CanExecute(object)"/>が呼び出された際に実行されます。
        /// </summary>
        /// <param name="parameter">
        /// コマンドのパラメータ。型が<seealso cref="T"/>とは異なる場合は既定値。
        /// </param>
        /// <returns>コマンドが実行可能ならtrue、不可ならfalse</returns>
        protected abstract bool CanExecute(T parameter);

        /// <summary>
        /// <seealso cref="ICommand.Execute(object)"/>が呼び出された際に実行されます。
        /// </summary>
        /// <param name="parameter">
        /// コマンドのパラメータ。型が<seealso cref="T"/>とは異なる場合は既定値。
        /// </param>
        protected abstract void Execute(T parameter);

        /// <summary>
        /// CanExecuteが変化したことを通知します。
        /// </summary>
        public void RaiseCanExecute()
        {
            this.dummyCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// コマンドを破棄します。
        /// </summary>
        public virtual void Dispose()
        {
            this._events.Clear();
        }
    }
}
