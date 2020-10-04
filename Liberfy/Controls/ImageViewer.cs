using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Liberfy
{
    /// <summary>
    /// 画像ビューア
    /// </summary>
    [TemplatePart(Name = "PART_Container", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_Image", Type = typeof(Image))]
    internal class ImageViewer : Control
    {
        private Panel _container;
        private Image _image;

        private bool _isZooming = false;
        private double _zoomLevel = double.NaN;
        private Size _imageSize = default;
        private double _offsetLeft = 0.0d;
        private double _offsetTop = 0.0d;
        private Size _previousViewSize;

        static ImageViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageViewer), new FrameworkPropertyMetadata(typeof(ImageViewer)));
        }

        /// <summary>
        /// 表示画像を取得または設定する。
        /// </summary>
        public ImageSource Source
        {
            get => (ImageSource)this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }

        /// <summary>
        /// <see cref="Source"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(
                nameof(Source), typeof(ImageSource),
                typeof(ImageViewer), new FrameworkPropertyMetadata(null, OnSourceChanged));

        /// <summary>
        /// <see cref="SourceProperty"/>変更時
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = (ImageViewer)d;

            bool _isLoading = false;
            if (e.NewValue is ImageSource imageSource)
            {
                if (imageSource is BitmapSource bitmapSource)
                {
                    if (bitmapSource.IsDownloading)
                    {
                        _isLoading = true;
                        bitmapSource.DownloadCompleted += viewer.OnImageDownloaded;
                    }
                }

                if (!_isLoading)
                {
                    viewer.OnImageLoaded(imageSource);
                }
            }
        }

        /// <summary>
        /// 拡大縮小方法を取得または設定する。
        /// </summary>
        public ZoomMode ZoomMode
        {
            get => (ZoomMode)this.GetValue(ZoomModeProperty);
            set => this.SetValue(ZoomModeProperty, value);
        }

        /// <summary>
        /// <see cref="ZoomMode"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty
            = DependencyProperty.Register(
                nameof(ZoomMode), typeof(ZoomMode),
                typeof(ImageViewer), new FrameworkPropertyMetadata(ZoomMode.AutoFit, OnZoomModeChanged));

        /// <summary>
        /// <see cref="ZoomMode"/>変更時
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnZoomModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// 画像のダウンロード完了時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImageDownloaded(object sender, EventArgs e)
        {
            var bitmapSource = (BitmapSource)sender;
            bitmapSource.DownloadCompleted -= this.OnImageDownloaded;

            this.OnImageLoaded(bitmapSource);
        }

        /// <summary>
        /// 画像の読み込み完了時
        /// </summary>
        /// <param name="imageSource"></param>
        private void OnImageLoaded(ImageSource imageSource)
        {
            this._isZooming = false;
            this._imageSize = new Size(
                (int)imageSource.Width,
                (int)imageSource.Height);

            this._zoomLevel = 1.0d;

            this.ArrangeImage();
        }

        /// <summary>
        /// テンプレート適用時
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var oldImage = this._image;
            if (oldImage != null)
            {
                oldImage.Source = null;
            }

            var newImage = (Image)this.GetTemplateChild("PART_Image");
            if (newImage != null)
            {
                newImage.Stretch = Stretch.Fill;
            }

            this._image = newImage;

            //var oldContainer = this._container;
            var container = (Panel)GetTemplateChild("PART_Container");

            this._container = container;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double GetImageScale()
        {
            var viewSize = this._container.RenderSize;
            var image = this._image.Source;

            double scale = Math.Max(
                image.Width / viewSize.Width,
                image.Height / viewSize.Height);

            return this._isZooming ? scale : Math.Max(1.0, scale);
        }

        /// <summary>
        /// 画像表示サイズが親要素のサイズより大きいか取得する。
        /// </summary>
        /// <returns></returns>
        private bool GetIsImageBiggerThanView()
        {
            var containerSize = this._container.RenderSize;
            var image = this._image.Source;

            return containerSize.Width < image.Width || containerSize.Height < image.Height;
        }

        /// <summary>
        /// 画像表示を調整する。
        /// </summary>
        private void ArrangeImage()
        {
            var image = this._image;
            if (image == null)
            {
                return;
            }

            var scallingMode = BitmapScalingMode.HighQuality;
            var prevRenderSize = image.RenderSize;
            var viewSize = this._container.RenderSize;
            ref var imageSize = ref this._imageSize;

            (double renderWidth, double renderHeight) = (imageSize.Width, imageSize.Height);
            double posX, posY;

            if (this._isZooming)
            {
                double zoomLevel = this._zoomLevel;
                renderWidth *= zoomLevel;
                renderHeight *= zoomLevel;

                if (zoomLevel >= 1.75d)
                {
                    scallingMode = BitmapScalingMode.NearestNeighbor;
                }

                var anchorPoint = this._movedOffset;
                var previousViewSize = this._previousViewSize;
                posX = CalcImagePosition(this._offsetLeft, viewSize.Width, renderWidth, anchorPoint.X, prevRenderSize.Width, previousViewSize.Width);
                posY = CalcImagePosition(this._offsetTop, viewSize.Height, renderHeight, anchorPoint.Y, prevRenderSize.Height, previousViewSize.Height);
            }
            else
            {
                if (this.GetIsImageBiggerThanView())
                {
                    double scale = this.GetImageScale();

                    renderWidth /= scale;
                    renderHeight /= scale;
                }

                posX = CalcImageCenterPosition(viewSize.Width, renderWidth);
                posY = CalcImageCenterPosition(viewSize.Height, renderHeight);
            }

            image.Width = renderWidth;
            image.Height = renderHeight;
            this.SetImagePosition(
                this._offsetLeft = posX,
                this._offsetTop = posY);

            if (RenderOptions.GetBitmapScalingMode(this._image) != scallingMode)
            {
                RenderOptions.SetBitmapScalingMode(this._image, scallingMode);
            }
        }

        /// <summary>
        /// 画像の表示位置を設定する。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetImagePosition(double x, double y)
        {
            this.SetImageXPosition(x);
            this.SetImageYPosition(y);
        }

        /// <summary>
        /// 画像のX位置を設定する。
        /// </summary>
        /// <param name="x"></param>
        private void SetImageXPosition(double x)
        {
            Canvas.SetLeft(this._image, x);
        }

        /// <summary>
        /// 画像のY位置を設定する。
        /// </summary>
        /// <param name="y"></param>
        private void SetImageYPosition(double y)
        {
            Canvas.SetTop(this._image, y);
        }

        /// <summary>
        /// サイズ変更時
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!this._isZooming)
            {
                this._zoomLevel = this.GetImageScale();
            }

            this._previousViewSize = sizeInfo.PreviousSize.IsEmpty ? sizeInfo.NewSize : sizeInfo.PreviousSize;
            this.ArrangeImage();
        }

        private const double ZoomCoe = 1.20d;

        /// <summary>
        /// マウス押下時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left)
            {
                this.StartImageMoving();
            }
        }

        /// <summary>
        /// マウスの押下解放時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            this.ReleaseMouseCapture();

            if (this._isImageMoving)
            {
                this.StopIamgeMoving();
            }
        }

        /// <summary>
        /// マウスホイール変更時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            double zoomLevel = this._zoomLevel;
            if (!this._isZooming)
            {
                this._isZooming = true;
                double scale = this.GetImageScale();
                if (scale > 1.0)
                {
                    zoomLevel = 1 / scale;
                }
            }

            if (e.Delta > 0)
            {
                zoomLevel *= ZoomCoe;
            }
            else if (e.Delta < 0)
            {
                zoomLevel /= ZoomCoe;
            }

            this._zoomLevel = zoomLevel;

            this._movedOffset = this.GetImagePoint(e.GetPosition(this._image));

            this.ArrangeImage();
        }

        /// <summary>
        /// マウス移動時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this._isImageMoving)
            {
                this.ApplyImageMoving();
            }
        }

        private Point _moveStartPosition = default;
        private double _movedOffsetX = double.NaN;
        private double _movedOffsetY = double.NaN;
        private Point _movedOffset = default;
        private bool _isImageMoving = false;

        /// <summary>
        /// 画像のドラッグを開始する。
        /// </summary>
        private void StartImageMoving()
        {
            this._isImageMoving = true;

            this._movedOffsetX = this._offsetLeft;
            this._movedOffsetY = this._offsetTop;

            this._moveStartPosition = Mouse.GetPosition(this);
            Mouse.Capture(this);
            this.Cursor = Cursors.ScrollAll;
        }

        /// <summary>
        /// ドラッグ中の画像表示を移動させる。
        /// </summary>
        private void ApplyImageMoving()
        {
            var position = Mouse.GetPosition(this);
            var delta = position - this._moveStartPosition;

            var image = this._image;
            double renderWidth = image.RenderSize.Width;
            double renderHeight = image.RenderSize.Height;

            var viewSize = this._container.RenderSize;

            if (viewSize.Width < renderWidth)
            {
                double posX = AlignmentImagePosition(this._offsetLeft + delta.X, renderWidth, viewSize.Width);

                this._movedOffsetX = posX;
                this.SetImageXPosition(posX);
            }

            if (viewSize.Height < renderHeight)
            {
                double posY = AlignmentImagePosition(this._offsetTop + delta.Y, renderHeight, viewSize.Height);

                this._movedOffsetY = posY;
                Canvas.SetTop(image, posY);
                this.SetImageYPosition(posY);
            }
        }

        /// <summary>
        /// 画像のドラッグを終了。
        /// </summary>
        private void StopIamgeMoving()
        {
            this._isImageMoving = false;
            this.Cursor = Cursors.Arrow;

            this._offsetLeft = this._movedOffsetX;
            this._offsetTop = this._movedOffsetY;
            this._movedOffset = this.GetImagePoint(Mouse.GetPosition(this._image));
        }

        /// <summary>
        /// 画像の表示位置を調整する。
        /// </summary>
        /// <remarks>
        /// X位置／Y位置共通の処理。
        /// </remarks>
        /// <param name="position"></param>
        /// <param name="renderSize"></param>
        /// <param name="viewSize"></param>
        /// <returns></returns>
        private static double AlignmentImagePosition(double position, double renderSize, double viewSize)
        {
            if (position > 0)
            {
                // 上・左の余白を詰める
                return 0;
            }
            else if ((position + renderSize) < viewSize)
            {
                // 下・右の余白を詰める
                return viewSize - renderSize;
            }

            return position;
        }

        /// <summary>
        /// 画像の位置計算を行う。
        /// </summary>
        /// <remarks>
        /// X位置、Y位置の共通処理。
        /// </remarks>
        /// <param name="prevPosition"></param>
        /// <param name="viewerSize"></param>
        /// <param name="renderSize"></param>
        /// <param name="position"></param>
        /// <param name="prevRenderSize"></param>
        /// <param name="prevViewerSize"></param>
        /// <returns></returns>
        private double CalcImagePosition(double prevPosition, double viewerSize, double renderSize, double position, double prevRenderSize, double prevViewerSize)
        {
            if (renderSize <= viewerSize)
            {
                return (viewerSize - renderSize) / 2.0d;
            }

            double offset = prevPosition;
            if (renderSize != prevRenderSize)
            {
                // 画像サイズ変更時
                offset -= (renderSize * position) - (prevRenderSize * position);
            }
            else
            {
                offset += (viewerSize - prevViewerSize) / 2;
            }

            return AlignmentImagePosition(offset, renderSize, viewerSize);
        }

        private double CalcImageCenterPosition(double viewSize, double renderSize)
        {
            return (viewSize - renderSize) / 2.0d;
        }

        /// <summary>
        /// 座標を0.0～1.0で取得する。
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point GetImagePoint(Point point)
        {
            var imageRenderSize = this._image.RenderSize;

            double normalizedWidth = imageRenderSize.Width - 1;
            double normalizedHeight = imageRenderSize.Height - 1;

            double x = Math.Max(0, Math.Min(normalizedWidth, point.X)) / normalizedWidth;
            double y = Math.Max(0, Math.Min(normalizedHeight, point.Y)) / normalizedHeight;

            return new Point(x, y);
        }
    }
}
