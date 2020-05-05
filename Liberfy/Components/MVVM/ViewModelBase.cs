using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    /// <summary>
    /// ViewModelのベースクラス
    /// </summary>
    internal abstract class ViewModelBase : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewModelBase()
        {
            this._commands = new Collection<ICommand>();
        }
        
        /// <summary>
        /// ViewModelに登録されたコマンド。
        /// </summary>
        private readonly ICollection<ICommand> _commands;

        /// <summary>
        /// 複数のプロパティの変更を通知する。
        /// </summary>
        /// <param name="propertyNames"></param>
        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            this.RaisePropertiesChanged((IEnumerable<string>)propertyNames);
        }

        /// <summary>
        /// 複数のプロパティの変更を通知する。
        /// </summary>
        /// <param name="propertyNames"></param>
        protected void RaisePropertiesChanged(IEnumerable<string> propertyNames)
        {
            foreach(var propertyName in propertyNames)
            {
                this.RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// ViewModelにコマンドを登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <param name="command">登録するコマンド</param>
        /// <returns>Command</returns>
        public Command RegisterCommand(Command command)
        {
            this._commands.Add(command);
            return command;
        }

        /// <summary>
        /// ViewModelにコマンドを登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <typeparam name="T"><see cref="Command{T}"/></typeparam>
        /// <param name="command">登録するコマンド</param>
        /// <returns>Command</returns>
        public Command<T> RegisterCommand<T>(Command<T> command)
        {
            this._commands.Add(command);
            return command;
        }

        /// <summary>
        /// コマンドを生成してViewModelに登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="hookRequerysuggested"></param>
        /// <returns></returns>
        public Command RegisterCommand(Action execute, Func<bool> canExecute = null, bool hookRequerysuggested = false)
        {
            var command = new DelegateCommand(execute, canExecute ?? DelegateCommand.EmptyCanExecute, hookRequerysuggested);
            return this.RegisterCommand(command);
        }

        /// <summary>
        /// コマンドを生成してViewModelに登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="isHookRequerySuggested"></param>
        /// <returns></returns>
        public Command<T> RegisterCommand<T>(Action<T> execute, Predicate<T> canExecute = null, bool isHookRequerySuggested = false)
        {
            var command = new DelegateCommand<T>(execute, canExecute ?? DelegateCommand<T>.EmptyCanExecute, isHookRequerySuggested);
            return this.RegisterCommand(command);
        }

        /// <summary>
        /// コマンドを生成してViewModelに登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="isHookRequerySuggested"></param>
        /// <returns></returns>
        public Command<T> CreateCommand<T>(Action<T> execute, Predicate<T> canExecute = null, bool isHookRequerySuggested = false)
        {
            var command = new DelegateCommand<T>(execute, canExecute ?? DelegateCommand<T>.EmptyCanExecute, isHookRequerySuggested);
            return this.RegisterCommand(command);
        }

        /// <summary>
        /// コマンドを生成してViewModelに登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="isHookRequerySuggested"></param>
        /// <returns></returns>
        public Command RegisterCommand(Func<Task> execute, Func<bool> canExecute = null, bool isHookRequerySuggested = false)
        {
            var command = new AsyncDelegateCommand(execute, canExecute ?? AsyncDelegateCommand.EmptyCanExecute, isHookRequerySuggested);
            return this.RegisterCommand(command);
        }

        /// <summary>
        /// コマンドを生成してViewModelに登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="isHookRequerySuggested"></param>
        /// <returns></returns>
        public Command<T> RegisterCommand<T>(Func<T, Task> execute, Predicate<T> canExecute = null, bool isHookRequerySuggested = false)
        {
            var command = new AsyncDelegateCommand<T>(execute, canExecute ?? AsyncDelegateCommand<T>.EmptyCanExecute, isHookRequerySuggested);
            return this.RegisterCommand(command);
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (var command in this._commands)
            {
                if (command is IDisposable disposableCommand)
                {
                    disposableCommand.Dispose();
                }
            }

            this._commands.Clear();
        }
    }
}
