using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	partial class NativeMethods
	{
		#region Shell32
		[DllImport("shell32", CharSet = CharSet.Unicode)]
		public static extern bool Shell_NotifyIcon(NIM dwMessage, [In] ref NOTIFYICONDATA lpData);
		#endregion
	}
}
