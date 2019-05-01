using System;
using System.Collections.Generic;
using System.Text;

namespace WpfMvvmToolkit
{
    public class DelegateCommand<T> : Command<T>
    {
        private Action<T> _execute;
        private Predicate<T> _canExecute;

        public DelegateCommand(Action<T> execute)
            : this(execute, DefaultCanExecute, false)
        {
        }

        public DelegateCommand(Action<T> execute, bool hookRequerySuggested)
            : this(execute, DefaultCanExecute, hookRequerySuggested)
        {
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
            : this(execute, canExecute, false)
        {
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute, bool hookRequerySuggested)
            : base(hookRequerySuggested)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        protected override bool CanExecute(T parameter) => this._canExecute.Invoke(parameter);

        protected override void Execute(T parameter) => this._execute.Invoke(parameter);

        public override void Dispose()
        {
            base.Dispose();

            this._execute = null;
            this._canExecute = null;
        }

        private static readonly Predicate<T> DefaultCanExecute = (_) => true;
    }
}
