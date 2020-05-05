using System;
using System.Windows.Input;

namespace WpfMvvmToolkit
{
    /// <summary>
    /// コマンド
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Command<T> : IDisposableCommand
    {
        private readonly bool _hookRequerySuggested;
        private EventHandler _dummyCanExecuteChangedHandler;
        private readonly WeakCollection<EventHandler> _events = new WeakCollection<EventHandler>();

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        protected Command()
        {
        }

        /// <summary>
        /// コマンドを生成する。
        /// </summary>
        /// <param name="hookRequerySuggested">CanExecute購読時にRequerySuggestedの購読を行うかどうかのフラグ</param>
        protected Command(bool hookRequerySuggested)
        {
            this._hookRequerySuggested = hookRequerySuggested;
        }

        /// <summary>
        /// CanExecuteChangedイベントの購読または購読解除を行う。
        /// </summary>
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                this._dummyCanExecuteChangedHandler += value;

                if (this._hookRequerySuggested)
                {
                    CommandManager.RequerySuggested += value;
                }

                this._events.Add(value);
            }
            remove
            {
                this._dummyCanExecuteChangedHandler -= value;

                if (this._hookRequerySuggested)
                {
                    CommandManager.RequerySuggested -= value;
                }

                this._events.Remove(value);
            }
        }

        /// <summary>
        /// コマンド実行検証の抽象メソッド
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected abstract bool CanExecute(T parameter);

        /// <summary>
        /// コマンドが実行可能か検証する。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter is T value ? value : default);
        }

        /// <summary>
        /// コマンド実行の抽象メソッド
        /// </summary>
        /// <param name="parameter"></param>
        protected abstract void Execute(T parameter);

        /// <summary>
        /// コマンドを実行する。
        /// </summary>
        /// <param name="parameter"></param>
        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter is T value ? value : default);
        }

        /// <summary>
        /// コマンドのCanExecuteを実行する。
        /// </summary>
        public void RaiseCanExecute() => this._dummyCanExecuteChangedHandler?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// インスタンス破棄の仮想メソッド
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            foreach (var weakEvent in this._events)
            {
                if (weakEvent.TryGetTarget(out var targetDelegate))
                {
                    this._dummyCanExecuteChangedHandler -= targetDelegate;
                }
            }

            this._events.Clear();
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Command()
        {
            this.Dispose(false);
        }
    }
}
