using System;
using System.Windows.Input;

namespace Liberfy
{
    /// <summary>
    /// コマンドを定義する際に基となるクラス。
    /// </summary>
    internal abstract class Command : Components.IDisposableCommand
    {
        private readonly bool hookRequerySuggested;
        private EventHandler dummyCanExecuteChanged;
        private readonly WeakCollection<EventHandler> _events = new WeakCollection<EventHandler>();

        /// <summary>
        /// <see cref="Command" />クラスの新しいインスタンスを生成します。
        /// </summary>
        protected Command()
        {
        }

        /// <summary>
        /// <see cref="Command"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="hookRequerySuggested">
        /// <seealso cref="CanExecuteChanged"/>へイベントが購読された際に、<see cref="CommandManager"/>.<seealso cref="RequerySuggested"/>への購読も行うかの設定
        /// </param>
        protected Command(bool hookRequerySuggested) : this()
        {
            this.hookRequerySuggested = hookRequerySuggested;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                dummyCanExecuteChanged += value;
                if (hookRequerySuggested)
                {
                    CommandManager.RequerySuggested += value;
                }
                _events.Add(value);
            }
            remove
            {
                dummyCanExecuteChanged -= value;
                if (hookRequerySuggested)
                {
                    CommandManager.RequerySuggested -= value;
                }
                _events.Remove(value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter);
        }

        /// <summary>
        /// <seealso cref="ICommand.CanExecute(object)"/>が呼び出された際に実行されます。
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ。</param>
        /// <returns></returns>
        protected abstract bool CanExecute(object parameter);

        /// <summary>
        /// <seealso cref="ICommand.Execute(object)"/>が呼び出された際に実行されます。
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ。</param>
        protected abstract void Execute(object parameter);

        /// <summary>
        /// CanExecuteが変化したことを通知します。
        /// </summary>
        public void RaiseCanExecute()
        {
            dummyCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// コマンドを破棄します。
        /// </summary>
        public virtual void Dispose()
        {
            _events.Clear();
        }
    }
}
