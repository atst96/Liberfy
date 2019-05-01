using Liberfy.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    internal class ViewModelBase : WpfMvvmToolkit.ViewModelBase
    {
        public ViewModelBase()
        {
            this.DialogService = new DialogService(this);
            this.WindowService = new WindowService(this);
            this._registeredCommands = new Collection<ICommand>();
        }
        
        private readonly ICollection<ICommand> _registeredCommands;

        public DialogService DialogService { get; private set; }

        public WindowService WindowService { get; private set; }
        
        [Obsolete]
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

        [Obsolete]
        public Command RegisterCommand(Action execute, Func<bool> canExecute, bool hookRequerysuggested = false)
        {
            var command = new DelegateCommand(execute, canExecute, hookRequerysuggested);

            this._registeredCommands.Add(command);

            return command;
        }

        [Obsolete]
        public Command<T> RegisterCommand<T>(Action<T> execute, Predicate<T> canExecute, bool hookRequerySuggested = false)
        {
            var command = new DelegateCommand<T>(execute, canExecute, hookRequerySuggested);

            this._registeredCommands.Add(command);

            return command;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.IsDisposed)
                return;

            foreach (var command in this._registeredCommands)
            {
                if (command is IDisposable disposableCommand)
                {
                    disposableCommand.Dispose();
                }
            }
            this._registeredCommands.Clear();

            this.DialogService.Dispose();
            this.DialogService = null;
        }
    }
}
