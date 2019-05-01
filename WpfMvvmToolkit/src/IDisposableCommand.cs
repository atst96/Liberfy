using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfMvvmToolkit
{
    public interface IDisposableCommand : ICommand, IDisposable
    {
    }
}
