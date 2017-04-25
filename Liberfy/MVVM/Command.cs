using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Liberfy
{
	abstract class Command : ICommand, IDisposable
	{
		readonly bool hookRequerySuggested;
		WeakCollection<EventHandler> _events = new WeakCollection<EventHandler>();

		protected Command() { }

		protected Command(bool hookRequerySuggested) : this()
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

	abstract class Command<T> : Command
	{
		protected Command() : base() { }

		protected Command(bool hookRequerySuggested)
			: base(hookRequerySuggested) { }

		public override bool CanExecute(object parameter)
		{
			return CanExecute(parameter.CastOrDefault<T>());
		}

		public override void Execute(object parameter)
		{
			Execute(parameter.CastOrDefault<T>());
		}

		public abstract bool CanExecute(T parameter);

		public abstract void Execute(T parameter);
	}
}
