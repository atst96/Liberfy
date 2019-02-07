using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    public class ApplicationStatus : NotificationObject
    {
        private bool _isAccountLoaded;
        public bool IsAccountLoaded
        {
            get => this._isAccountLoaded;
            set => this.SetProperty(ref this._isAccountLoaded, value);
        }
    }
}
