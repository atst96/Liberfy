using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	public class DragDropHelper : IDisposable
	{
		private IDragDropHelper _helper;
		private IntPtr _hwnd;

		#region Constants

		private static readonly Guid CLSID_DragDropHelper = new Guid("4657278A-411B-11d2-839A-00C04FD918D0");
		private static readonly Guid IID_IDropTargetHelper = new Guid(GUID_IDragDropHelper);
		private const int CLSCTX_INPROC_SERVER = 0x1;

		private const string GUID_IDragDropHelper = "4657278B-411B-11D2-839A-00C04FD918D0";

		#endregion Constants

		#region Win32Api

		[DllImport("ole32.dll")]
		private extern static int CoCreateInstance(
			[In] ref Guid rclsid,
			[In, MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
			[In] int dwClsContext,
			[In] ref Guid riid,
			[Out, MarshalAs(UnmanagedType.IUnknown)] out object ppv
		);

		[DllImport("user32.dll")]
		private static extern uint RegisterClipboardFormat(string lpszFormat);

		#endregion Win32Api

		#region Interfaces

		[ComImport, Guid(GUID_IDragDropHelper)]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IDragDropHelper
		{
			int DragEnter(
				[In] IntPtr hwndTarget,
				[In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObject,
				[In] ref POINT ppt,
				[In] int dwEffect
			);

			int DragLeave();

			int DragOver(
				[In] ref POINT ppt,
				[In] int dwEffect
			);

			int Drop(
				[In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObject,
				[In] ref POINT ppt,
				[In] int dwEffect
			);

			int Show([In] bool fShow);
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT
		{
			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 1044)]
		private struct DROPDESCRIPTION
		{
			public int type;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szMessage;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szInsert;
		}

		#endregion Interfaces

		public DragDropHelper()
		{
			SetHelper();
		}

		public DragDropHelper(IntPtr hwnd) : this()
		{
			_hwnd = hwnd;
		}

		private void SetHelper()
		{
			var rclsid = CLSID_DragDropHelper;
			var riid = IID_IDropTargetHelper;

			CoCreateInstance(ref rclsid, null, CLSCTX_INPROC_SERVER, ref riid, out object obj);
			_helper = (IDragDropHelper)obj;
		}

		public void SetHandle(IntPtr handle)
		{
			_hwnd = handle;
		}

		public void Dispose()
		{
			_hwnd = IntPtr.Zero;
			Marshal.FinalReleaseComObject(_helper);
			_helper = null;
		}


		#region Base functions

		public void DragEnter(IDataObject pDataObject, int x, int y, int effect)
		{
			var ppt = new POINT(x, y);
			_helper.DragEnter(_hwnd, pDataObject, ref ppt, effect);
		}

		public void DragOver(int x, int y, int effect)
		{
			var ppt = new POINT(x, y);
			_helper.DragOver(ref ppt, effect);
		}

		public void DragLeave() => _helper.DragLeave();

		public void Drop(IDataObject pDataObject, int x, int y, int effect)
		{
			var ppt = new POINT(x, y);
			_helper.Drop(pDataObject, ref ppt, effect);
		}

		public void Show(bool show) => _helper.Show(show);

		public static void SetDescription(IDataObject dataObject, DropImageType type, string message, string insert)
		{
			var formatETC = new FORMATETC
			{
				cfFormat = (short)RegisterClipboardFormat("DropDescription"),
				dwAspect = DVASPECT.DVASPECT_CONTENT,
				lindex = -1,
				ptd = IntPtr.Zero,
				tymed = TYMED.TYMED_HGLOBAL
			};

			var dropDescription = new DROPDESCRIPTION
			{
				type = (int)type,
				szMessage = message,
				szInsert = insert,
			};

			var pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DROPDESCRIPTION)));

			try
			{
				Marshal.StructureToPtr(dropDescription, pointer, false);

				var medium = new STGMEDIUM
				{
					pUnkForRelease = null,
					tymed = TYMED.TYMED_HGLOBAL,
					unionmember = pointer,
				};

				dataObject.SetData(ref formatETC, ref medium, true);
			}
			catch
			{
				Marshal.FreeHGlobal(pointer);
				// throw;
			}
		}

		#endregion Base functions


		#region Functions for WPF

		public void DragEnter(System.Windows.IDataObject dataObject, System.Windows.Point point, System.Windows.DragDropEffects effect)
		{
			DragEnter((IDataObject)dataObject, (int)point.X, (int)point.Y, (int)effect);
		}

		public void DragOver(System.Windows.Point point, System.Windows.DragDropEffects effect)
		{
			DragOver((int)point.X, (int)point.Y, (int)effect);
		}

		public void Drop(System.Windows.IDataObject dataObject, System.Windows.Point point, System.Windows.DragDropEffects effect)
		{
			Drop((IDataObject)dataObject, (int)point.X, (int)point.Y, (int)effect);
		}

		public static void SetDescription(System.Windows.IDataObject dataObject, DropImageType type, string message, string insert = null)
		{
			SetDescription((IDataObject)dataObject, type, message, insert);
		}

		public static void SetDescription(System.Windows.IDataObject dataObject, System.Windows.DragDropEffects effect, string message, string insert = null)
		{
			DropImageType type;

			switch (effect)
			{
				case System.Windows.DragDropEffects.Copy:
					type = DropImageType.Copy;
					break;

				case System.Windows.DragDropEffects.Link:
					type = DropImageType.Link;
					break;

				case System.Windows.DragDropEffects.Move:
					type = DropImageType.Move;
					break;

				case System.Windows.DragDropEffects.None:
					type = DropImageType.None;
					break;

				default:
					type = DropImageType.NoImage;
					break;
			}

			SetDescription((IDataObject)dataObject, type, message, insert);
		}

		#endregion Functions for WPF
	}

	public enum DropImageType
	{
		Invalid = -1,
		None = 0,
		Copy = 1,
		Move = 2,
		Link = 4,
		Label = 6,
		Warning = 7,
		NoImage = 8
	}
}

