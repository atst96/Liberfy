using Liberfy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfMvvmToolkit
{
    public abstract class Command<T> : IDisposableCommand
    {
        private readonly bool _hookRequerySuggested;
        private EventHandler _dummyCanExecuteChangedHandler;
        private readonly WeakCollection<EventHandler> _events = new WeakCollection<EventHandler>();

        protected Command()
        {
        }

        protected Command(bool hookRequerySuggested)
        {
            this._hookRequerySuggested = hookRequerySuggested;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                this._dummyCanExecuteChangedHandler += value;

                if (this._hookRequerySuggested)
                {
                    CommandManager.RequerySuggested += value;
                }

                this._events.Add(value);
            }
            remove
            {
                this._dummyCanExecuteChangedHandler -= value;

                if (this._hookRequerySuggested)
                {
                    CommandManager.RequerySuggested -= value;
                }

                this._events.Remove(value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter is T value ? value : default);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter is T value ? value : default);
        }

        protected abstract bool CanExecute(T parameter);

        protected abstract void Execute(T parameter);

        public bool TryExecute(T parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.Execute(parameter);

                return true;
            }

            return false;
        }

        public void RaiseCanExecute()
        {
            this._dummyCanExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Dispose()
        {
            this._events.Clear();
        }
    }
}
