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
		public enum NotifyIconIcon : int
		{
			None = 0x00000000,
			Info = 0x00000001,
			Warning = 0x00000002,
			Error = 0x00000003
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct NOTIFYICONDATA
		{
			public int cbSize;
			public IntPtr hWnd;
			public int uID;
			public NIF uFlags;
			public int uCallbackMessage;
			public IntPtr hIcon;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szTip;
			public int dwState;
			public int dwStateMask;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szInfo;
			public int uVersion;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szInfoTitle;
			public NIIF dwInfoFlags;
			public Guid guidItem;
			public IntPtr hBalloonIcon;
		}

		public enum NIF : int
		{
			MESSAGE = 0x00000001,
			ICON = 0x00000002,
			TIP = 0x00000004,
			STATE = 0x00000008,
			INFO = 0x00000010,
			GUID = 0x00000020,
			REALTIME = 0x00000040,
			SHOWTIP = 0x00000080
		}

		public enum NIM : int
		{
			ADD = 0x00000000,
			MODIFY = 0x00000001,
			DELETE = 0x00000002,
			SETFOCUS = 0x00000003,
			SETVERSION = 0x00000004
		}

		public enum NIIF : int
		{
			NONE = 0x00000000,
			INFO = 0x00000001,
			WARNING = 0x00000002,
			ERROR = 0x00000003,
			USER = 0x00000004,
			NOSOUND = 0x00000010,
			LARGE_ICON = 0x00000020,
			RESPECT_QUIET_TIME = 0x00000080,
			ICON_MASK = 0x0000000F
		}
	}
}
