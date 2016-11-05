using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Liberfy.NativeMethods;
using static System.Reflection.BindingFlags;

namespace Liberfy
{
	public sealed class NotifyIcon : IDisposable
	{
		private int id;
		private NativeWindow niNativeWindow;
		private System.Windows.Forms.NotifyIcon notifyIcon;

		private static Type niType;
		private static FieldInfo nifiId;
		private static FieldInfo nifiWindow;
		static NotifyIcon()
		{
			niType = typeof(System.Windows.Forms.NotifyIcon);

			BindingFlags flags = GetField | NonPublic | Instance;
			nifiId = niType.GetField("id", flags);
			nifiWindow = niType.GetField("window", flags);
		}

		public NotifyIcon()
		{
			notifyIcon = new System.Windows.Forms.NotifyIcon();

			id = (int)nifiId.GetValue(notifyIcon);
			niNativeWindow = (NativeWindow)nifiWindow.GetValue(notifyIcon);
		}

		public bool Visible
		{
			get { return notifyIcon.Visible; }
			set { notifyIcon.Visible = value; }
		}

		public Icon Icon
		{
			get { return notifyIcon.Icon; }
			set { notifyIcon.Icon = value; }
		}

		public ContextMenu ContextMenu
		{
			get { return notifyIcon.ContextMenu; }
			set { notifyIcon.ContextMenu = value; }
		}

		public ContextMenuStrip ContextMenuStrip
		{
			get { return notifyIcon.ContextMenuStrip; }
			set { notifyIcon.ContextMenuStrip = value; }
		}

		public event EventHandler OnClick
		{
			add { notifyIcon.Click += value; }
			remove { notifyIcon.Click -= value; }
		}

		public event EventHandler OnDoubleClick
		{
			add { notifyIcon.DoubleClick += value; }
			remove { notifyIcon.DoubleClick -= value; }
		}

		public event EventHandler OnBalloonTipClick
		{
			add { notifyIcon.BalloonTipClicked += value; }
			remove { notifyIcon.BalloonTipClicked -= value; }
		}

		public event EventHandler OnBalloonTipClose
		{
			add { notifyIcon.BalloonTipClosed += value; }
			remove { notifyIcon.BalloonTipClosed -= value; }
		}

		public event EventHandler OnBalloonTipShow
		{
			add { notifyIcon.BalloonTipShown += value; }
			remove { notifyIcon.BalloonTipShown -= value; }
		}

		public event EventHandler Disposed
		{
			add { notifyIcon.Disposed += value; }
			remove { notifyIcon.Disposed -= value; }
		}

		public void ShowBalloonTip(string tipTitle, string tipText, ToolTipIcon tipIcon)
			=> notifyIcon.ShowBalloonTip(0, tipTitle, tipText, tipIcon);

		public bool ShowBalloonTip(string tipTitle, string tipText, Icon tipIcon, bool noSound = true)
		{
			var nid = new NOTIFYICONDATA()
			{
				cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA)),
				hIcon = Icon?.Handle ?? IntPtr.Zero,
				hWnd = niNativeWindow.Handle,
				szInfoTitle = tipTitle,
				szInfo = tipText,
				uFlags = NIF.ICON | NIF.TIP | NIF.INFO,
				uID = id,
			};

			if (tipIcon != null)
			{
				nid.hBalloonIcon = tipIcon.Handle;
				nid.dwInfoFlags |= NIIF.USER | NIIF.LARGE_ICON;
			}

			if (noSound)
			{
				nid.dwInfoFlags |= NIIF.NOSOUND;
			}

			return Shell_NotifyIcon(NIM.MODIFY, ref nid);
		}

		#region IDispose

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (notifyIcon != null)
				{
					notifyIcon.Dispose();
					notifyIcon = null;
				}
			}

			id = 0;
			niNativeWindow = null;
		}

		~NotifyIcon()
		{
			Dispose(false);
		}

		#endregion
	}
}
