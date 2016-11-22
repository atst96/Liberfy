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
		#region Kernel32

		[DllImport("kernel32")]
		public static extern IntPtr GetCurrentThreadId();

		#endregion

		#region Shell32

		[DllImport("shell32", CharSet = CharSet.Unicode)]
		public static extern bool Shell_NotifyIcon(
			[In] NIM dwMessage, 
			[In] ref NOTIFYICONDATA lpData
		);

		#endregion

		#region User32 

		[DllImport("user32")]
		public static extern IntPtr CallNextHookEx(
			[In] IntPtr hhk,
			[In] int nCode,
			[In] IntPtr wParam,
			[In] IntPtr lParam
		);

		[DllImport("user32")]
		public static extern bool GetWindowRect(
			[In] IntPtr hWnd,
			[Out] out RECT lpRect
		);

		[DllImport("user32")]
		public static extern IntPtr GetWindowLong(
			[In] IntPtr hWnd,
			[In] GWL nIndex
		);

		[DllImport("user32")]
		public static extern bool SetWindowPos(
			[In] IntPtr hWnd, 
			[In] IntPtr hWndInsertAfter,
			[In] int X,
			[In] int Y,
			[In] int cx, 
			[In] int cy, 
			[In] SWP uFlags
		);

		[DllImport("user32")]
		public static extern bool UnhookWindowsHookEx(
			[In] IntPtr hhk
		);

		[DllImport("user32", CharSet = CharSet.Unicode)]
		public static extern ID MessageBox(
			[In] IntPtr hWnd, 
			[In] string lpText, 
			[In] string lpCaption, 
			[In] MB uType
		);

		[DllImport("user32")]
		public static extern IntPtr SetWindowsHookEx(
			[In] WH idHook, 
			[In] HHOOK hookProc,
			[In] IntPtr hInstance, 
			[In] IntPtr dwThreadId
		);

		[DllImport("user32")]
		public static extern bool MoveWindow(
			[In] IntPtr hWnd,
			[In] int X,
			[In] int Y,
			[In] int nWidth,
			[In] int nHeight,
			[In] bool bRepaint
		);
		#endregion
	}
}
