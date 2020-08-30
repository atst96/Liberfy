using Liberfy.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Liberfy
{
    /// <summary>
    /// プロフィール画像のキャッシュを行うクラス
    /// </summary>
    internal class ProfileImageCache
    {
        private readonly ConcurrentDictionary<IUserInfo, ImageCacheInfo> _images;

        /// <summary>コンストラクタ</summary>
        /// <param name="dbConnection">データベース接続</param>
        public ProfileImageCache()
        {
            this._images = new ConcurrentDictionary<IUserInfo, ImageCacheInfo>();
        }

        /// <summary>デバッグ情報に出力する</summary>
        /// <param name="text"></param>
        private void Log(string text)
        {
            System.Diagnostics.Debug.WriteLine($"[{nameof(ProfileImageCache)}] {text}");
        }

        /// <summary>ユーザ情報からキャッシュ情報を取得または生成する</summary>
        /// <param name="userInfo">ユーザ情報</param>
        /// <returns>キャッシュ情報</returns>
        public ImageCacheInfo GetCacheInfo(IUserInfo userInfo)
        {
            return this._images.AddOrUpdate(userInfo, this.CreateCacheReference, this.UpdateCacheReference);
        }

        /// <summary>画像のリサイズ処理を行う</summary>
        /// <param name="srcBitmap">ソースとなる画像</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <returns><リサイズ後の画像</returns>
        private static Bitmap ResizeImage(Bitmap srcBitmap, int width, int height)
        {
            var destImage = new Bitmap(width, height);
            using var g = Graphics.FromImage(destImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(srcBitmap, 0, 0, width, height);

            return destImage;
        }

        /// <summary>画像データを縮小する</summary>
        /// <param name="refStream">画像データ</param>
        /// <param name="imageSize">画像の最大サイズ</param>
        private static void ShrinkImageData(ref MemoryStream refStream, int imageSize = 128)
        {
            using var srcImage = new Bitmap(refStream);

            if (srcImage.Width <= imageSize && srcImage.Height <= imageSize)
            {
                refStream.Position = 0;
                return;
            }

            double maxScale = Math.Max(
                (double)srcImage.Width / imageSize,
                (double)srcImage.Height / imageSize);

            int resizeWidth = (int)(srcImage.Width / maxScale);
            int resizeHeight = (int)(srcImage.Height / maxScale);

            using var destImage = ResizeImage(srcImage, resizeWidth, resizeHeight);
            var imageStream = new MemoryStream();

            System.Diagnostics.Debug.WriteLine($"[{nameof(ProfileImageCache)}] Image resized. width: {resizeWidth}, height: {resizeHeight}");

            destImage.Save(imageStream, ImageFormat.Tiff);
            refStream.Dispose();

            refStream = imageStream;
        }

        /// <summary>DB検索時のパラメータを生成する</summary>
        /// <param name="userInfo">ユーザ情報</param>
        /// <returns>パラメータ</returns>
        private static Dictionary<string, object> CreateParameter(IUserInfo userInfo)
        {
            return new Dictionary<string, object>(5)
            {
                ["user_id"] = userInfo.Id,
                ["service_id"] = (int)userInfo.Service,
                ["host_url"] = userInfo.Instance?.Host ?? "",
            };
        }

        /// <summary>データベースからキャッシュデータ取得を試行する</summary>
        /// <param name="params">パラメータ</param>
        /// <param name="imageFileName">ファイル名</param>
        /// <param name="imageData">画像データの出力</param>
        /// <returns>キャッシュデータの有無</returns>
        private bool TryFindCache(IDictionary<string, object> @params, string imageFileName, out byte[] imageData)
        {
            imageData = null;
            return false;
        }

        /// <summary>キャッシュ内容をデータベースに書き込む</summary>
        /// <param name="params"></param>
        private void WriteCache(IDictionary<string, object> @params)
        {
        }

        /// <summary>画像をダウンロードする</summary>
        /// <param name="userInfo">ユーザ情報</param>
        /// <param name="imageStream">画像データの出力</param>
        /// <returns>ダウンロード成功かどうか</returns>
        private bool TryDownloadImage(IUserInfo userInfo, out MemoryStream imageStream)
        {
            try
            {
                var request = WebRequest.CreateHttp(userInfo.ProfileImageUrl);

                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                var dataStream = new MemoryStream();
                stream.CopyTo(dataStream);

                ProfileImageCache.ShrinkImageData(ref dataStream);

                imageStream = dataStream;

                return true;
            }
            catch
            {
                imageStream = null;
                return false;
            }
        }

        private void AssertThread(ImageCacheInfo cacheInfo, int taskId)
        {
            if (cacheInfo.GetCurrentTaskId() != taskId)
            {
                throw new TaskCanceledException();
            }
        }

        /// <summary>キャッシュ情報から画像データを取得する</summary>
        /// <param name="cacheInfo">キャッシュ情報</param>
        /// <param name="userInfo">ユーザ情報</param>
        /// <returns>画像データ</returns>
        private BitmapImage LoadImage(ImageCacheInfo cacheInfo, IUserInfo userInfo)
        {
            int taskId = cacheInfo.GetCurrentTaskId();

            this.AssertThread(cacheInfo, taskId);
            var imageUrl = userInfo.ProfileImageUrl;

            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            var imageFileName = Path.GetFileName(imageUrl);
            var @params = CreateParameter(userInfo);

            this.AssertThread(cacheInfo, taskId);
            if (this.TryFindCache(@params, imageFileName, out var imageData))
            {
                this.AssertThread(cacheInfo, taskId);
                return ImageUtil.CreateBitmapImage(new MemoryStream(imageData));
            }

            this.AssertThread(cacheInfo, taskId);
            if (this.TryDownloadImage(userInfo, out var imageStream))
            {
                this.AssertThread(cacheInfo, taskId);
                //@params.Add("filename", imageFileName);
                //@params.Add("image_data", imageStream.ToArray());

                //this.WriteCache(@params);

                this.AssertThread(cacheInfo, taskId);

                return ImageUtil.CreateBitmapImage(imageStream);
            }

            this.AssertThread(cacheInfo, taskId);

            return null;
        }

        /// <summary>キャッシュ情報に画像データを格納する</summary>
        /// <param name="cacheInfo"></param>
        /// <param name="userInfo"></param>
        private void SetImage(ImageCacheInfo cacheInfo, IUserInfo userInfo)
        {
            int taskId = cacheInfo.CreateTaskId();

            Task.Run(() => this.LoadImage(cacheInfo, userInfo))
                .ContinueWith(
                    task => cacheInfo.Image = task.Result,
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>キャッシュ情報を生成する</summary>
        /// <param name="userInfo">ユーザ情報</param>
        /// <returns>キャッシュ情報</returns>
        private ImageCacheInfo CreateCache(IUserInfo userInfo)
        {
            var cache = new ImageCacheInfo
            {
                FileName = Path.GetFileName(userInfo.ProfileImageUrl),
            };

            this.SetImage(cache, userInfo);

            return cache;
        }

        /// <summary>キャッシュの弱参照を作成する</summary>
        /// <param name="userInfo">ユーザ情報</param>
        /// <returns>弱参照</returns>
        private ImageCacheInfo CreateCacheReference(IUserInfo userInfo)
        {
            return this.CreateCache(userInfo);
        }

        /// <summary>キャッシュ情報を更新する</summary>
        /// <param name="userInfo">ユーザ情報</param>
        /// <param name="cacheInfo">キャッシュ情報</param>
        /// <returns>キャッシュ情報</returns>
        private ImageCacheInfo UpdateCache(IUserInfo userInfo, ImageCacheInfo cacheInfo)
        {
            if (cacheInfo.FileName != Path.GetFileName(userInfo.ProfileImageUrl))
            {
                this.SetImage(cacheInfo, userInfo);
            }

            return cacheInfo;
        }

        /// <summary>弱参照中のキャッシュ情報を更新する</summary>
        /// <param name="userInfo"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        private ImageCacheInfo UpdateCacheReference(IUserInfo userInfo, ImageCacheInfo reference)
        {
            this.UpdateCache(userInfo, reference);

            return reference;
        }
    }
}
