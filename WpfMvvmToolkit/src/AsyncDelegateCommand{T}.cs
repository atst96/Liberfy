using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvmToolkit
{
    public class AsyncDelegateCommand<T> : Command<T>
    {
        private Func<T, Task> _execute;
        private Predicate<T> _canExecute;

        public AsyncDelegateCommand(Func<T, Task> execute)
            : this(execute, EmptyCanExecute, false)
        {
        }

        public AsyncDelegateCommand(Func<T, Task> execute, bool hookRequerySuggested)
            : this(execute, EmptyCanExecute, hookRequerySuggested)
        {
        }

        public AsyncDelegateCommand(Func<T, Task> execute, Predicate<T> canExecute)
            : this(execute, canExecute, false)
        {
        }

        public AsyncDelegateCommand(Func<T, Task> execute, Predicate<T> canExecute, bool hookRequerySuggested)
            : base(hookRequerySuggested)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        protected override bool CanExecute(T parameter)
        {
            return this._canExecute.Invoke(parameter);
        }

        protected override async void Execute(T parameter)
        {
            await this._execute.Invoke(parameter);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._execute = null;
            this._canExecute = null;
        }

        internal static bool EmptyCanExecute(T parameter) => true;
    }
}
