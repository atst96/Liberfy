using Liberfy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal interface ITextEntityBuilder
    {
        IEnumerable<EntityBase> Build();
    }
}
