using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvmToolkit.src
{
    public class AsyncDelegateCommand<T> : Command<T>
    {
        private Func<T, Task> _execute;
        private Func<T, bool> _canExecute;

        public AsyncDelegateCommand(Func<T, Task> execute)
            : this(execute, DefaultCanExecute, false)
        {
        }

        public AsyncDelegateCommand(Func<T, Task> execute, bool hookRequerySuggested)
            : this(execute, DefaultCanExecute, hookRequerySuggested)
        {
        }

        public AsyncDelegateCommand(Func<T, Task> execute, Func<T, bool> canExecute)
            : this(execute, canExecute, false)
        {
        }

        public AsyncDelegateCommand(Func<T, Task> execute, Func<T, bool> canExecute, bool hookRequerySuggested)
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

        public override void Dispose()
        {
            base.Dispose();

            this._execute = null;
            this._canExecute = null;
        }

        private static readonly Func<T, bool> DefaultCanExecute = (_) => true;
    }
}
