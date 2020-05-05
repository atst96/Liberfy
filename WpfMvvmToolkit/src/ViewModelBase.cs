using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfMvvmToolkit
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public ViewModelBase()
        {
            this._commands = new Collection<ICommand>();
        }

        private readonly ICollection<ICommand> _commands;

        public event PropertyChangedEventHandler PropertyChanged;

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

        protected void SetPropertyForce<T>(ref T refValue, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            refValue = newValue;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            if (this.PropertyChanged != null)
            {
                foreach (var name in propertyNames)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public Command CreateCommand(Action execute, Func<bool> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new DelegateCommand(execute, canExecute ?? DelegateCommand.EmptyCanExecute, isHookRequerySuggested));
        }

        public Command<T> CreateCommand<T>(Action<T> execute, Predicate<T> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new DelegateCommand<T>(execute, canExecute ?? DelegateCommand<T>.EmptyCanExecute, isHookRequerySuggested));
        }

        public Command CreateCommand(Func<Task> execute, Func<bool> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new AsyncDelegateCommand(execute, canExecute ?? AsyncDelegateCommand.EmptyCanExecute, isHookRequerySuggested));
        }

        public Command<T> CreateCommand<T>(Func<T, Task> execute, Predicate<T> canExecute = null, bool isHookRequerySuggested = false)
        {
            return this.RegisterCommand(new AsyncDelegateCommand<T>(execute, canExecute ?? AsyncDelegateCommand<T>.EmptyCanExecute, isHookRequerySuggested));
        }

        public Command RegisterCommand(Command command)
        {
            this._commands.Add(command);
            return command;
        }

        public Command<T> RegisterCommand<T>(Command<T> command)
        {
            this._commands.Add(command);
            return command;
        }

        internal virtual void OnInitialized() { }

        internal virtual bool CanClose() => true;

        internal virtual void OnClosed() { }

        public virtual void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;

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
