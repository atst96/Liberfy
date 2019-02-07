using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Liberfy.ViewModel
{
    internal class ViewModelBase : INotifyPropertyChanged, IViewModelBase, IDisposable
    {
        public ViewModelBase()
        {
            _dialogService = new DialogService(this);
            _registeredCommands = new Collection<ICommand>();
        }

        private bool _isDisposed;
        private DialogService _dialogService;
        // ビューモデルと同時に破棄するコマンド
        private readonly ICollection<ICommand> _registeredCommands;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// ビューモデルと関連付けられたダイアログ機能提供サービスです。
        /// </summary>
        public DialogService DialogService => _dialogService;

        /// <summary>
        /// ビューモデルが破棄済みかどうかを返します。
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// フィールドに値を設定し、変更がある場合はプロパティ名で変更通知を行います。
        /// </summary>
        /// <typeparam name="T">フィールドの型</typeparam>
        /// <param name="refVal">値の設定先</param>
        /// <param name="value">値</param>
        /// <param name="propertyName">（自動設定）変更を通知するプロパティ名</param>
        /// <returns>値の変更の有無</returns>
        protected bool SetProperty<T>(ref T refVal, T value, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(refVal, value))
            {
                refVal = value;
                RaisePropertyChanged(propertyName);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// フィールドに値を設定し、変更がある場合はプロパティならびに指定のコマンドの変更通知を行います。
        /// </summary>
        /// <typeparam name="T">フィールドの型</typeparam>
        /// <param name="refVal">値の設定先</param>
        /// <param name="value">値</param>
        /// <param name="command">値に変更がある場合に参照するコマンド</param>
        /// <param name="propertyName">（自動設定）変更を通知するプロパティ名</param>
        /// <returns>値の変更の有無</returns>
        protected bool SetProperty<T>(ref T refVal, T value, Command command, [CallerMemberName] string propertyName = "")
        {
            if (SetProperty(ref refVal, value, propertyName))
            {
                command?.RaiseCanExecute();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// フィールドに値を設定し、変更がある場合はプロパティならびに指定のコマンドの変更通知を行います。
        /// </summary>
        /// <typeparam name="T">フィールドの型</typeparam>
        /// <param name="refVal">値の設定先</param>
        /// <param name="value">値</param>
        /// <param name="command">値に変更がある場合に参照するコマンド</param>
        /// <param name="propertyName">（自動設定）変更を通知するプロパティ名</param>
        /// <returns>値の変更の有無</returns>
        protected bool SetProperty<T>(ref T refVal, T value, Command<T> command, [CallerMemberName] string propertyName = "")
        {
            if (SetProperty(ref refVal, value, propertyName))
            {
                command?.RaiseCanExecute();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// フィールドに値を設定し、常にプロパティの変更通知を行います。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refValue"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetPropertyForce<T>(ref T refValue, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            refValue = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティの変更通知を行います。
        /// </summary>
        /// <param name="propertyName">変更を通知するプロパティ名</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 複数のプロパティの変更通知を行います。
        /// </summary>
        /// <param name="propertyNames">変更を通知するプロパティ名の配列</param>
        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            if (this.PropertyChanged != null)
                foreach (var name in propertyNames)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// ViewModelと同時に破棄されるコマンドを登録します。
        /// </summary>
        /// <param name="command">コマンド登録するコマンド</param>
        /// <returns>Command</returns>
        public Command RegisterCommand(Command command)
        {
            this._registeredCommands.Add(command);
            return command;
        }

        public Command<T> RegisterCommand<T>(Command<T> command)
        {
            this._registeredCommands.Add(command);
            return command;
        }

        [Obsolete]
        /// <summary>
        /// ViewModelと同時に破棄されるコマンドを登録します。
        /// </summary>
        /// <param name="execute">コマンドのExecute処理</param>
        /// <param name="hookRequerySuggested">RequerySuggestedイベントに登録するかを設定します</param>
        /// <returns>Command</returns>
        public Command RegisterCommand(Action execute, bool hookRequerySuggested = false)
        {
            var command = new DelegateCommand(execute, hookRequerySuggested);
            _registeredCommands.Add(command);

            return command;
        }

        [Obsolete]
        /// <summary>
        /// ビューモデルと同時に破棄されるコマンドを登録します。
        /// </summary>
        /// <param name="execute">コマンドのExecute処理</param>
        /// <param name="canExecute">コマンドのCanExecute処理</param>
        /// <param name="hookRequerysuggested">RequerySuggestedイベントに登録するかを設定します</param>
        /// <returns>Command</returns>
        public Command RegisterCommand(Action execute, Func<bool> canExecute, bool hookRequerysuggested = false)
        {
            var command = new DelegateCommand(execute, canExecute, hookRequerysuggested);
            _registeredCommands.Add(command);

            return command;
        }

        [Obsolete]
        /// <summary>
        /// ビューモデルと同時に破棄されるコマンドを登録します。
        /// </summary>
        /// <typeparam name="T">コマンドのパラメータの型</typeparam>
        /// <param name="execute">コマンドのExecute処理</param>
        /// <param name="hookRequerySuggested">RequerySuggestedイベントに登録するかを設定します</param>
        /// <returns>Command</returns>
        public Command<T> RegisterCommand<T>(Action<T> execute, bool hookRequerySuggested = false)
        {
            var command = new DelegateCommand<T>(execute, hookRequerySuggested);
            _registeredCommands.Add(command);

            return command;
        }

        [Obsolete]
        /// <summary>
        /// ビューモデルと同時に破棄されるコマンドを登録します。
        /// </summary>
        /// <typeparam name="T">コマンドのパラメータの型</typeparam>
        /// <param name="execute">コマンドのExecute処理</param>
        /// <param name="canExecute">コマンドのCanExecute処理</param>
        /// <param name="hookRequerySuggested">RequerySuggestedイベントに登録するかを設定します</param>
        /// <returns>Command</returns>
        public Command<T> RegisterCommand<T>(Action<T> execute, Predicate<T> canExecute, bool hookRequerySuggested = false)
        {
            var command = new DelegateCommand<T>(execute, canExecute, hookRequerySuggested);
            _registeredCommands.Add(command);

            return command;
        }

        /// <summary>
        /// ビューが初期化された際に呼び出されます。
        /// </summary>
        internal virtual void OnInitialized() { }

        /// <summary>
        /// ビューが閉じられる際に呼び出されます。
        /// </summary>
        /// <returns>ビューを閉じて良いかの値</returns>
        internal virtual bool CanClose() => true;

        /// <summary>
        /// ビューが閉じられた際に呼び出されます。
        /// </summary>
        internal virtual void OnClosed() { }

        /// <summary>
        /// ビューモデルを破棄します。
        /// </summary>
        public virtual void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            foreach (var command in _registeredCommands)
            {
                if (command is IDisposable _dCmd)
                {
                    _dCmd.Dispose();
                }
            }
            _registeredCommands.Clear();

            _dialogService.Dispose();
            _dialogService = null;
        }
    }

    internal interface IViewModelBase
    {
        DialogService DialogService { get; }
    }
}
