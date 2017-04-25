using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Liberfy.ViewModel
{
	class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		public ViewModelBase() : base()
		{
			_dialogService = new DialogService(this);
			_registeredCommands = new Collection<Command>();
		}

		private readonly ICollection<Command> _registeredCommands;

		private DialogService _dialogService;
		public DialogService DialogService => _dialogService;

		public event PropertyChangedEventHandler PropertyChanged;

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

		protected bool SetProperty<T1, T2>(ref T1 refVal, T1 value, Command<T2> command, [CallerMemberName] string propertyName = "")
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

		protected void SetPropertyForce<T>(ref T refValue, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			refValue = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Command RegisterReleasableCommand(Action execute, bool hookRequerySuggested = false)
		{
			var command = new DelegateCommand(execute, hookRequerySuggested);
			_registeredCommands.Add(command);

			return command;
		}

		public Command RegisterReleasableCommand(Action execute, Predicate<object> canExecute, bool hookRequerysuggested = false)
		{
			var command = new DelegateCommand(execute, canExecute, hookRequerysuggested);
			_registeredCommands.Add(command);

			return command;
		}

		public Command<T> RegisterReleasableCommand<T>(Action<T> execute, bool hookRequerySuggested = false)
		{
			var command = new DelegateCommand<T>(execute, hookRequerySuggested);
			_registeredCommands.Add(command);

			return command;
		}

		public Command<T> RegisterReleasableCommand<T>(Action<T> execute, Predicate<T> canExecute, bool hookRequerySuggested = false)
		{
			var command = new DelegateCommand<T>(execute, canExecute, hookRequerySuggested);
			_registeredCommands.Add(command);

			return command;
		}

		public bool UnregisterReleasableCommand(Command command)
		{
			return _registeredCommands.Remove(command);
		}

		internal virtual void OnInitialized() { }

		internal virtual bool CanClose() => true;

		internal virtual void OnClosed() { }

		public virtual void Dispose()
		{
			foreach (var command in _registeredCommands)
			{
				command.Dispose();
			}
			_registeredCommands.Clear();

			_dialogService.Dispose();
			_dialogService = null;
		}
	}

	interface IViewModelBase
	{
		DialogService DialogService { get; }
	}
}
