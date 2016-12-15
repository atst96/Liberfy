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

		public DelegateCommand(Action action) : base(false)
		{
			_execute = action;
			_canExecute = DefaultCanExecute;
		}

		public DelegateCommand(Action action, bool hookRequerySuggested)
			: base(hookRequerySuggested)
		{
			_execute = action;
			_canExecute = DefaultCanExecute;
		}

		public DelegateCommand(Action action, Predicate<object> predicate = null)
			: base(false)
		{
			_execute = action;
			_canExecute = predicate ?? DefaultCanExecute;
		}

		public DelegateCommand(Action action, Predicate<object> predicate = null, bool hookRequerySuggested = false)
			: base(hookRequerySuggested)
		{
			_execute = action;
			_canExecute = predicate ?? DefaultCanExecute;
		}

		public override bool CanExecute(object parameter)
		{
			return _canExecute(parameter);
		}

		public override void Execute(object parameter)
		{
			_execute();
		}

		static bool DefaultCanExecute(object p) => true;
	}

	class DelegateCommand<T> : Command<T>
	{
		Action<T> _execute;
		Predicate<T> _canExecute;

		public DelegateCommand(Action<T> action, bool hookRequerySuggested = false)
			: base(hookRequerySuggested)
		{
			_execute = action;
			_canExecute = DefaultCanExecute;
		}

		public DelegateCommand(Action<T> action, Predicate<T> predicate = null, bool hookRequerySuggested = false)
			: base(hookRequerySuggested)
		{
			_execute = action;
			_canExecute = DefaultCanExecute;
		}

		public override bool CanExecute(T parameter)
		{
			return _canExecute(parameter);
		}

		public override void Execute(T parameter)
		{
			_execute(parameter);
		}

		static bool DefaultCanExecute(T parameter) => true;
	}
}
