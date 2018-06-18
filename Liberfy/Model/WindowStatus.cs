using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
    [DataContract]
    internal class WindowStatus
    {
        [DataMember(Name = "top")]
        public double? Top { get; set; }

        [DataMember(Name = "left")]
        public double? Left { get; set; }
        
        [DataMember(Name = "width")]
        public double? Width { get; set; }

        [DataMember(Name = "height")]
        public double? Height { get; set; }

        [DataMember(Name = "window.state")]
        public WindowState? State { get; set; }

        public static WindowState ConvertWindowState(System.Windows.WindowState state)
        {
            switch (state)
            {
                case System.Windows.WindowState.Maximized:
                    return WindowState.Maximized;

                case System.Windows.WindowState.Normal:
                    return WindowState.Normal;

                case System.Windows.WindowState.Minimized:
                    return WindowState.Minimized;

                default:
                    throw new NotImplementedException();
            }
        }

        public static System.Windows.WindowState ConvertWindowState(WindowState state)
        {
            switch (state)
            {
                case WindowState.Maximized:
                    return System.Windows.WindowState.Maximized;

                case WindowState.Normal:
                    return System.Windows.WindowState.Normal;

                case WindowState.Minimized:
                    return System.Windows.WindowState.Minimized;

                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal enum WindowState
    {
        [EnumMember(Value = "minimized")]
        Minimized,

        [EnumMember(Value = "maximized")]
        Maximized,

        [EnumMember(Value = "normal")]
        Normal,
    }
}
