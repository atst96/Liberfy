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
            : this(execute, EmptyCanExecute, false)
        {
        }

        public DelegateCommand(Action execute, bool hookRequerySuggested)
            : this(execute, EmptyCanExecute, hookRequerySuggested)
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

        protected override bool CanExecute(object parameter)
        {
            return this._canExecute.Invoke();
        }

        protected override void Execute(object parameter)
        {
            this._execute.Invoke();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._execute = null;
            this._canExecute = null;
        }

        public static bool EmptyCanExecute() => true;
    }
}
