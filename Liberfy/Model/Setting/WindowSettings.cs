using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Settings
{
    [DataContract]
    internal class WindowSettings
    {
        [Key("main")]
        [DataMember(Name = "main")]
        private WindowStatus _main;
        [IgnoreDataMember]
        public WindowStatus Main
        {
            get => this._main ?? (this._main = new WindowStatus());
            set => this._main = value;
        }
    }
}
