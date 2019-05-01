using System;
using System.Collections.Generic;
using System.Text;

namespace WpfMvvmToolkit
{
    public class DelegateCommand : Command
    {
        private Action _execute;
        private Func<bool> _canExecute;

        public DelegateCommand(Action execute)
            : this(execute, DefaultCanExecute, false)
        {
        }

        public DelegateCommand(Action execute, bool hookRequerySuggested)
            : this(execute, DefaultCanExecute, hookRequerySuggested)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : this(execute, canExecute, false)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute, bool hookRequerySuggested)
            : base(hookRequerySuggested)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        protected override bool CanExecute(object parameter) => this._canExecute.Invoke();

        protected override void Execute(object parameter) => this._execute.Invoke();

        public override void Dispose()
        {
            base.Dispose();

            this._execute = null;
            this._canExecute = null;
        }

        private static readonly Func<bool> DefaultCanExecute = () => true;
    }
}
