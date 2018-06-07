using SocialApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.ViewModel.Timeline
{
    internal interface ITimeline
    {
        FluidCollection<ColumnBase> Columns { get; }
        void Load();
        void Unload();
    }
}
