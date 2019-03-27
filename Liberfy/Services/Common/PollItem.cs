using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class PollItem : NotificationObject
    {
        public string _text;
        public string Text
        {
            get => this._text;
            set => this.SetProperty(ref this._text, value);
        }
    }
}
