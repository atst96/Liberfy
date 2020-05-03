using Liberfy.Components;
using Livet;
using Livet.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    internal class ViewModelBase : Livet.ViewModel
    {
        public ViewModelBase()
        {
            this._registeredCommands = new Collection<ICommand>();
        }
        
        private readonly ICollection<ICommand> _registeredCommands;

        public Command RegisterCommand(Action execute, Func<bool> canExecute, bool hookRequerysuggested = false)
        {
            var command = new DelegateCommand(execute, canExecute, hookRequerysuggested);

            this._registeredCommands.Add(command);

            return command;
        }

        public Command<T> RegisterCommand<T>(Action<T> execute, Predicate<T> canExecute, bool hookRequerySuggested = false)
        {
            var command = new DelegateCommand<T>(execute, canExecute, hookRequerySuggested);

            this._registeredCommands.Add(command);

            return command;
        }

        protected override void Dispose(bool disposed)
        {
            base.Dispose(disposed);

            if (this.IsDisposed)
            {
                return;
            }

            foreach (var command in this._registeredCommands)
            {
                if (command is IDisposable disposableCommand)
                {
                    disposableCommand.Dispose();
                }
            }
            this._registeredCommands.Clear();
        }

        public bool IsDisposed { get; private set; }

        protected bool SetProperty<T>(ref T refValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (!object.Equals(refValue, newValue))
            {
                refValue = newValue;
                this.RaisePropertyChanged(propertyName);

                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool SetProperty<T>(ref T refValue, T newValue, Command command, [CallerMemberName] string propertyName = "")
        {
            if (this.SetProperty(ref refValue, newValue, propertyName))
            {
                command?.RaiseCanExecute();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool SetProperty<T>(ref T refValue, T newValue, Command<T> command, [CallerMemberName] string propertyName = "")
        {
            if (this.SetProperty(ref refValue, newValue, propertyName))
            {
                command?.RaiseCanExecute();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void SetPropertyForce<T>(ref T refValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            refValue = newValue;
            this.RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            this.RaisePropertiesChanged(propertyNames);
        }

        public Command CreateCommand(Action execute, Func<bool> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new DelegateCommand(execute, canExecute ?? DelegateCommand.DefaultCanExecute, isHookRequerySuggested));
        }

        public Command<T> CreateCommand<T>(Action<T> execute, Predicate<T> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new DelegateCommand<T>(execute, canExecute ?? DelegateCommand<T>.DefaultCanExecute, isHookRequerySuggested));
        }

        public Command CreateCommand(Func<Task> execute, Func<bool> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new AsyncDelegateCommand(execute, canExecute ?? AsyncDelegateCommand.DefaultCanExecute, isHookRequerySuggested));
        }

        public Command<T> CreateCommand<T>(Func<T, Task> execute, Predicate<T> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new AsyncDelegateCommand<T>(execute, canExecute ?? AsyncDelegateCommand<T>.DefaultCanExecute, isHookRequerySuggested));
        }

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

        internal virtual void OnInitialized() { }

        internal virtual bool CanClose() => true;

        internal virtual void OnClosed() { }
    }
}
