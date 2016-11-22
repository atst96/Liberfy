using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Liberfy.NativeMethods;

namespace Liberfy
{
	class MessageBox
	{
		public MessageBox() { }

		public MessageBox(IntPtr hWnd)
		{
			this.hWnd = hWnd;
		}

		private IntPtr hWnd;
		private IntPtr hook;

		public void SetWindowHandle(IntPtr hWnd) => this.hWnd = hWnd;

		public bool CenterOwner { get; } = true;

		public MsgBoxResult Show(string text, string caption = null, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
		{
			if (hWnd != IntPtr.Zero && CenterOwner)
			{
				IntPtr hInst = GetWindowLong(hWnd, GWL.HINSTANCE);
				IntPtr thrId = GetCurrentThreadId();
				hook = SetWindowsHookEx(WH.CBT, HookPrc, hInst, thrId);
			}

			return (MsgBoxResult)MessageBox(hWnd, text, caption, (MB)buttons | (MB)icon | (MB)flags);
		}

		public static MsgBoxResult Show(IntPtr hWnd, string text, string caption = null, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
		{
			return (MsgBoxResult)MessageBox(hWnd, text, caption, (MB)buttons | (MB)icon | (MB)flags);
		}

		private IntPtr HookPrc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode == (int)HCBT.ACTIVATE)
			{
				RECT parentRect, msgBoxRect;
				IntPtr res;
				int x, y;

				GetWindowRect(hWnd, out parentRect);
				GetWindowRect(wParam, out msgBoxRect);

				x = (parentRect.left + (parentRect.right - parentRect.left) / 2) - ((msgBoxRect.right - msgBoxRect.left) / 2);
				y = (parentRect.top + (parentRect.bottom - parentRect.top)  / 2) - ((msgBoxRect.bottom - msgBoxRect.top) / 2);

				SetWindowPos(wParam, IntPtr.Zero, x, y, 0, 0, SWP.NOSIZE | SWP.NOZORDER | SWP.NOACTIVATE);

				res = CallNextHookEx(hWnd, nCode, wParam, lParam);

				UnhookWindowsHookEx(hook);
				hook = IntPtr.Zero;

				return res;
			}
			else
			{
				return CallNextHookEx(hook, nCode, wParam, lParam);
			}
		}
	}

	[Flags]
	enum MsgBoxButtons : int
	{
		AbortRetryIgnore = MB.MB_ABORTRETRYIGNORE,
		CancelTryContinue = MB.MB_CANCELTRYCONTINUE,
		Help = MB.MB_HELP,
		Ok = MB.MB_OK,
		OkCancel = MB.MB_OKCANCEL,
		RetryCancel = MB.MB_RETRYCANCEL,
		YesNo = MB.MB_YESNO,
		YesNoCancel = MB.MB_YESNOCANCEL,
	}

	enum MsgBoxIcon : int
	{
		Exclamation = MB.MB_ICONEXCLAMATION,
		Warning = MB.MB_ICONWARNING,
		Information = MB.MB_ICONINFORMATION,
		Asterisk = MB.MB_ICONASTERISK,
		Question = MB.MB_ICONQUESTION,
		Stop = MB.MB_ICONSTOP,
		Error = MB.MB_ICONERROR,
		Hand = MB.MB_ICONHAND,
	}

	[Flags]
	enum MsgBoxFlags : int
	{
		DefButton1 = MB.MB_DEFBUTTON1,
		DefButton2 = MB.MB_DEFBUTTON2,
		DefButton3 = MB.MB_DEFBUTTON3,
		DefButton4 = MB.MB_DEFBUTTON4,

		AppModal = MB.MB_APPMODAL,
		SystemModal = MB.MB_SYSTEMMODAL,
		TaskModal = MB.MB_TASKMODAL,

		DefaultDesktopOnly = MB.MB_DEFAULT_DESKTOP_ONLY,
		Right = MB.MB_RIGHT,
		RTLReading = MB.MB_RTLREADING,
		SetForeground = MB.MB_SETFOREGROUND,
		TopMost = MB.MB_TOPMOST,
		ServiceNotification = MB.MB_SERVICE_NOTIFICATION,
	}

	enum MsgBoxResult : int
	{
		Abort = ID.ABORT,
		Cancel = ID.CANCEL,
		Continue = ID.CONTINUE,
		Ignore = ID.IGNORE,
		No = ID.NO,
		Ok = ID.OK,
		Retry = ID.RETRY,
		TryAgain = ID.TRYAGAIN,
		Yes = ID.YES
	}
}
