using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Liberfy
{
    internal interface IAccountCommandGroup
    {
        ICommand Repost { get; }
        ICommand Like { get; }
        ICommand DisplayUserDetails { get; }
    }
}
