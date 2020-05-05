using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvmToolkit
{
    public class AsyncDelegateCommand : Command
    {
        private Func<Task> _execute;
        private Func<bool> _canExecute;

        public AsyncDelegateCommand(Func<Task> execute)
            : this(execute, EmptyCanExecute, false)
        {
        }

        public AsyncDelegateCommand(Func<Task> execute, bool hookRequerySuggested)
            : this(execute, EmptyCanExecute, hookRequerySuggested)
        {
        }

        public AsyncDelegateCommand(Func<Task> execute, Func<bool> canExecute)
            : this(execute, canExecute, false)
        {
        }

        public AsyncDelegateCommand(Func<Task> execute, Func<bool> canExecute, bool hookRequerySuggested)
            : base(hookRequerySuggested)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        protected override bool CanExecute(object parameter) => this._canExecute.Invoke();

        protected override async void Execute(object parameter)
        {
            await this._execute.Invoke();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._execute = null;
            this._canExecute = null;
        }

        internal static bool EmptyCanExecute() => true;
    }
}
