using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class DelegateCommand : Command
	{
		Action _execute;
		Predicate<object> _canExecute;

		public DelegateCommand(Action action)
			: this(action, null, false) { }

		public DelegateCommand(Action action, bool hookRequerySuggested = false)
			: this(action, null, hookRequerySuggested) { }

		public DelegateCommand(Action action, Predicate<object> predicate = null, bool hookRequerySuggested = false)
			: base(hookRequerySuggested)
		{
			_execute = action;
			_canExecute = predicate ?? DefaultCanExecute;
		}

		public override bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke(parameter) ?? true;
		}

		public override void Execute(object parameter)
		{
			_execute?.Invoke();
		}

		public override void Dispose()
		{
			_execute = null;
			_canExecute = null;

			base.Dispose();
		}

		static bool DefaultCanExecute(object p) => true;
	}

	class DelegateCommand<T> : Command<T>
	{
		Action<T> _execute;
		Predicate<T> _canExecute;

		public DelegateCommand(Action<T> action)
			: this(action, null, false) { }

		public DelegateCommand(Action<T> action, bool hookRequerySuggested = false)
			: this(action, null, hookRequerySuggested) { }

		public DelegateCommand(Action<T> action, Predicate<T> predicate = null, bool hookRequerySuggested = false)
			: base(hookRequerySuggested)
		{
			_execute = action;
			_canExecute = predicate ?? DefaultCanExecute;

			new WeakReference(_execute);
			new WeakReference(_canExecute);
		}

		public override bool CanExecute(T parameter)
		{
			return _canExecute?.Invoke(parameter) ?? true;
		}

		public override void Execute(T parameter)
		{
			_execute?.Invoke(parameter);
		}

		public override void Dispose()
		{
			_execute = null;
			_canExecute = null;

			base.Dispose();
		}

		static bool DefaultCanExecute(T parameter) => true;
	}
}
