using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Liberfy
{
	public abstract class Command : ICommand, IDisposable
	{
		readonly bool hookRequerySuggested;
		WeakCollection<EventHandler> _events = new WeakCollection<EventHandler>();

		public Command() { }

		public Command(bool hookRequerySuggested) : this()
		{
			this.hookRequerySuggested = hookRequerySuggested;
		}

		private EventHandler dummyCanExecuteChanged;

		public event EventHandler CanExecuteChanged
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

		public abstract bool CanExecute(object parameter);

		public abstract void Execute(object parameter);

		public void RaiseCanExecute()
		{
			dummyCanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public virtual void Dispose()
		{
			_events.Clear();
		}
	}

	public abstract class Command<T> : ICommand, IDisposable
	{
		readonly bool hookRequerySuggested;
		WeakCollection<EventHandler> _events = new WeakCollection<EventHandler>();

		public Command() { }

		public Command(bool hookRequerySuggested) : this()
		{
			this.hookRequerySuggested = hookRequerySuggested;
		}

		private EventHandler dummyCanExecuteChanged;

		public event EventHandler CanExecuteChanged
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

		public abstract bool CanExecute(T parameter);

		public abstract void Execute(T parameter);

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute(parameter.CastOrDefault<T>());
		}

		void ICommand.Execute(object parameter)
		{
			Execute(parameter.CastOrDefault<T>());
		}

		public void RaiseCanExecute()
		{
			dummyCanExecuteChanged.Invoke(this, EventArgs.Empty);
		}

		public virtual void Dispose()
		{
			_events.Clear();
		}
	}
}
