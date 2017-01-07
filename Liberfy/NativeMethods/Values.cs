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
		[Flags]
		public enum MB : int
		{
			MB_ABORTRETRYIGNORE = 0x00000002,
			MB_CANCELTRYCONTINUE = 0x00000006,
			MB_HELP = 0x00004000,
			MB_OK = 0x00000000,
			MB_OKCANCEL = 0x00000001,
			MB_RETRYCANCEL = 0x00000005,
			MB_YESNO = 0x00000004,
			MB_YESNOCANCEL = 0x00000003,

			MB_ICONEXCLAMATION = 0x00000030,
			MB_ICONWARNING = 0x00000030,
			MB_ICONINFORMATION = 0x00000040,
			MB_ICONASTERISK = 0x00000040,
			MB_ICONQUESTION = 0x00000020,
			MB_ICONSTOP = 0x00000010,
			MB_ICONERROR = 0x00000010,
			MB_ICONHAND = 0x00000010,

			MB_DEFBUTTON1 = 0x00000000,
			MB_DEFBUTTON2 = 0x00000100,
			MB_DEFBUTTON3 = 0x00000200,
			MB_DEFBUTTON4 = 0x00000300,

			MB_APPMODAL = 0x00000000,
			MB_SYSTEMMODAL = 0x00001000,
			MB_TASKMODAL = 0x00002000,

			MB_DEFAULT_DESKTOP_ONLY = 0x00020000,
			MB_RIGHT = 0x00080000,
			MB_RTLREADING = 0x00100000,
			MB_SETFOREGROUND = 0x00010000,
			MB_TOPMOST = 0x00040000,
			MB_SERVICE_NOTIFICATION = 0x00200000
		}

		public enum ID : int
		{
			ABORT = 3,
			CANCEL = 2,
			CONTINUE = 11,
			IGNORE = 5,
			NO = 7,
			OK = 1,
			RETRY = 4,
			TRYAGAIN = 10,
			YES = 6,
		}

		[Flags]
		public enum GWL : int
		{
			EXSTYLE = -20,
			HINSTANCE = -6,
			HWNDPARENT = -8,
			ID = -12,
			STYLE = -16,
			USERDATA = -12,
			WNDPROC = -4,

			//DLGPROC = DWLP_MESSAGESULT + sizeof(LRESULT)
			MSGRESULT = 0,
			//USER = DWLP_DLGPROC + sizeof(DLGPROC)
		}

		[Flags]
		public enum WH : int
		{
			CALLWNDPROC = 4,
			CALLWNDPROCRET = 12,
			CBT = 5,
			DEBUG = 9,
			FOREGROUNDIFLE = 11,
			GETMESSAGE = 3,
			JOURNALPLAYBACK = 1,
			JOURNALRECORD = 0,
			KEYBOARD = 2,
			KEYBOARD_ALL = 13,
			MOUSE = 7,
			MOUSE_ALL = 14,
			MSGFILTER = -1,
			SHELL = 10,
			SYSMSGFILTER = 6
		}

		[Flags]
		public enum HCBT : int
		{
			ACTIVATE = 5,
			CLICKSKIPPED = 6,
			CREATEWND = 3,
			DESTROYWND = 4,
			KEYSKIPPED = 7,
			MINMAX = 1,
			MOVESIZE = 0,
			QS = 2,
			SETFOCUS = 9,
			SYSCOMMAND = 8
		}

		[Flags]
		public enum SWP : int
		{
			ASYNCWINDOWPOS = 0x4000,
			DEFERERASE = 0x2000,
			DRAWFRAME = 0x0020,
			FRAMECHANGED = 0x0020,
			HIDEWINDOW = 0x0080,
			NOACTIVATE = 0x0010,
			NOCOPYBITS = 0x0100,
			NOMOVE = 0x0002,
			NOOWNERZORDER = 0x0200,
			NOREDRAW = 0x0008,
			NOREPOSITION = 0x0200,
			NOSENDCHANGING = 0x0400,
			NOSIZE = 0x0001,
			NOZORDER = 0x0004,
			SHOWWINDOW = 0x0040
		}

		public struct RECT
		{
			public RECT(int a)
			{
				left = top = right = bottom = a;
			}

			public RECT(int l, int t, int r, int b)
			{
				left = l;
				top = t;
				right = r;
				bottom = b;
			}

			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		public delegate IntPtr HHOOK(int nCode, IntPtr wParam, IntPtr lParam);
	}
}
