using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Liberfy
{
    /// <summary>
    /// ドラッグ＆ドロップ操作をいい感じにするやつ
    /// </summary>
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

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
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

        /// <summary>
        /// DragDropHelperを生成する。
        /// </summary>
        public DragDropHelper()
        {
            this._helper = CreateDragDropHelperShellInstance();
        }

        /// <summary>
        /// DragDropHelperを生成する。
        /// </summary>
        /// <param name="hwnd">ウィンドウハンドル</param>
        public DragDropHelper(IntPtr hwnd) : this()
        {
            this._hwnd = hwnd;
        }

        /// <summary>
        /// DragDropTargetHelperのインスタンスを生成する。
        /// </summary>
        /// <returns></returns>
        private static IDragDropHelper CreateDragDropHelperShellInstance()
        {
            var rclsid = CLSID_DragDropHelper;
            var riid = IID_IDropTargetHelper;

            CoCreateInstance(ref rclsid, null, CLSCTX_INPROC_SERVER, ref riid, out object obj);
            return (IDragDropHelper)obj;
        }

        /// <summary>
        /// ウィンドウハンドルを設定する。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        public void SetHandle(IntPtr handle)
        {
            this._hwnd = handle;
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            this._hwnd = IntPtr.Zero;

            // COMオブジェクトを解放
            Marshal.FinalReleaseComObject(this._helper);
            this._helper = null;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~DragDropHelper()
        {
            this.Dispose(false);
        }

        #region Base functions

        /// <summary>
        /// ドラッグ＆ドロップ操作を開始する。
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="x">マウスポインタのX座標</param>
        /// <param name="y">マウスポインタのY座標</param>
        /// <param name="effect"></param>
        public void DragEnter(IDataObject dataObject, int x, int y, int effect)
        {
            var point = new POINT(x, y);
            this._helper.DragEnter(this._hwnd, dataObject, ref point, effect);
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作のマウスポインタ移動。
        /// </summary>
        /// <param name="x">マウスポインタのX座標</param>
        /// <param name="y">マウスポインタのY座標</param>
        /// <param name="effect"></param>
        public void DragOver(int x, int y, int effect)
        {
            var point = new POINT(x, y);
            this._helper.DragOver(ref point, effect);
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作を終了する。
        /// </summary>
        public void DragLeave() => this._helper.DragLeave();

        /// <summary>
        /// ドラッグ＆ドロップ操作のドロップ。
        /// </summary>
        /// <param name="dataObject">D&Dデータ</param>
        /// <param name="x">マウスポインタのX座標</param>
        /// <param name="y">マウスポインタのY座標</param>
        /// <param name="effect"></param>
        public void Drop(IDataObject dataObject, int x, int y, int effect)
        {
            var ppt = new POINT(x, y);
            this._helper.Drop(dataObject, ref ppt, effect);
        }

        public void Show(bool show) => this._helper.Show(show);

        /// <summary>
        /// ドラッグ＆ドロップ操作時のラベルを設定する。
        /// </summary>
        /// <param name="dataObject">D&Dデータ</param>
        /// <param name="type">ドラッグ操作時に表示する画像</param>
        /// <param name="message">ドラッグ操作時に表示するメッセージ</param>
        /// <param name="insert">ドラッグ操作時に表示するメッセージの補足</param>
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

        /// <summary>
        /// ドラッグ＆ドロップ操作の開始。
        /// </summary>
        /// <param name="dataObject">DataObject</param>
        /// <param name="point">マウスポインタの座標</param>
        /// <param name="effect"></param>
        public void DragEnter(System.Windows.IDataObject dataObject, System.Windows.Point point, System.Windows.DragDropEffects effect)
        {
            this.DragEnter((IDataObject)dataObject, (int)point.X, (int)point.Y, (int)effect);
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作のマウスポインタ移動。
        /// </summary>
        /// <param name="point">マウスポインタの座標</param>
        /// <param name="effect"></param>
        public void DragOver(System.Windows.Point point, System.Windows.DragDropEffects effect)
        {
            this.DragOver((int)point.X, (int)point.Y, (int)effect);
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作のドロップ。
        /// </summary>
        /// <param name="dataObject">DataObject</param>
        /// <param name="point">マウスポインタの座標</param>
        /// <param name="effect"></param>
        public void Drop(System.Windows.IDataObject dataObject, System.Windows.Point point, System.Windows.DragDropEffects effect)
        {
            this.Drop((IDataObject)dataObject, (int)point.X, (int)point.Y, (int)effect);
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作時のラベルを表示する。
        /// </summary>
        /// <param name="dataObject">DataObject</param>
        /// <param name="type">ドラッグ操作時に表示する画像</param>
        /// <param name="message">ドラッグ操作時に表示するメッセージ</param>
        /// <param name="insert">ドラッグ操作時に表示するメッセージの補足</param>
        public static void SetDescription(System.Windows.IDataObject dataObject, DropImageType type, string message, string insert = null)
        {
            SetDescription((IDataObject)dataObject, type, message, insert);
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作時のラベルを表示する。
        /// </summary>
        /// <param name="dataObject">DataObject</param>
        /// <param name="type">ドラッグ操作時に表示する画像</param>
        /// <param name="message">ドラッグ操作時に表示するメッセージ</param>
        /// <param name="insert">ドラッグ操作時に表示するメッセージの補足</param>
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
        NoImage = 8,
    }
}

