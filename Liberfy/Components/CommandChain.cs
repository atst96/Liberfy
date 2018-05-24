using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Component
{
    internal struct CommandChain
    {
        public CommandChain(Func<bool> canExecute)
        {
            this.CanExecute = canExecute;
        }

        public Func<bool> CanExecute { get; }

        public Command Exec(Action action)
        {
            return new DelegateCommand(action, this.CanExecute);
        }
    }

    internal struct CommandChain<T>
    {
        public CommandChain(Predicate<T> canExecute)
        {
            this.CanExecute = canExecute;
        }

        public Predicate<T> CanExecute { get; }

        public Command Exec(Action<T> action)
        {
            return new DelegateCommand<T>(action, this.CanExecute);
        }
    }
}
