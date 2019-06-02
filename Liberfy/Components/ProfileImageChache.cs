using Liberfy.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Liberfy
{
    internal class ProfileImageCache
    {
        private readonly ConcurrentDictionary<IUserInfo, WeakReference<ImageCacheInfo>> _images = new ConcurrentDictionary<IUserInfo, WeakReference<ImageCacheInfo>>();
        private readonly Database _dbConnection;
        private bool _isLoadTimelineMode = false;
        private SQLiteTransaction _sqlTransaction;

        public ProfileImageCache(Database dbConnection)
        {
            this._dbConnection = dbConnection;

            this.InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (this._dbConnection != null)
            {
                var tables = this._dbConnection.EnumerateTableNames();

                if (!tables.Contains(Database.TableNameCollection.PorfileImageCache))
                {
                    this._dbConnection.ExecuteNonQuery(Database.QueryCollection.CreateProfileImageCacheTable);
                }
            }
        }

        public void BeginLoadTimelineMode()
        {
            if (this._isLoadTimelineMode)
            {
                return;
            }

            this._isLoadTimelineMode = true;
            this._sqlTransaction = this._dbConnection?.BeginTransaction();
        }

        public void EndLoadTimelineMode()
        {
            if (!this._isLoadTimelineMode)
            {
                return;
            }

            this._isLoadTimelineMode = false;

            if (this._sqlTransaction != null)
            {
                this._sqlTransaction.Commit();
                this._sqlTransaction.Dispose();
            }
        }

        public ImageCacheInfo GetCacheInfo(IUserInfo userInfo)
        {
            var reference = this._images.AddOrUpdate(userInfo, this.CreateCacheReference, this.UpdateCacheReference);

            if (reference.TryGetTarget(out var cacheInfo))
            {
                return cacheInfo;
            }

            throw new NotImplementedException();
        }

        private static Bitmap ResizeImage(Bitmap srcBitmap, int width, int height)
        {
            var destImage = new Bitmap(width, height);

            using (var g = Graphics.FromImage(destImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(srcBitmap, 0, 0, width, height);
            }

            return destImage;
        }

        private static void ShrinkImageData(ref MemoryStream refStream, int imageSize = 128)
        {
            using (var srcImage = new Bitmap(refStream))
            {
                if (srcImage.Width <= imageSize && srcImage.Height <= imageSize)
                {
                    refStream.Position = 0;
                    return;
                }

                int resizeWidth, resizeHeight;

                if (srcImage.Width > srcImage.Height)
                {
                    resizeHeight = (int)(imageSize * (srcImage.Height / (double)srcImage.Width));
                    resizeWidth = imageSize;
                }
                else if (srcImage.Width < srcImage.Height)
                {
                    resizeWidth = (int)(imageSize * (srcImage.Width / (double)srcImage.Height));
                    resizeHeight = imageSize;
                }
                else
                {
                    resizeWidth = imageSize;
                    resizeHeight = imageSize;
                }

                using (var srcStream = refStream)
                using (var destImage = ResizeImage(srcImage, resizeWidth, resizeHeight))
                {
                    var imageStream = new MemoryStream();
                    destImage.Save(imageStream, ImageFormat.Tiff);

                    refStream = imageStream;
                }
            }
        }

        private static Dictionary<string, object> CreateParameter(IUserInfo userInfo)
        {
            return new Dictionary<string, object>(5)
            {
                ["user_id"] = userInfo.Id,
                ["service_id"] = (int)userInfo.Service,
                ["host_url"] = userInfo.Instance?.Host ?? "",
            };
        }

        private bool TryFindCache(IDictionary<string, object> @params, string imageFileName, out byte[] imageData)
        {
            var database = this._dbConnection;

            if (database == null || !database.IsConnected)
            {
                imageData = null;
                return false;
            }

            using (var query = database.ExecuteReader(Database.QueryCollection.SelectProfileImageCacheData, @params))
            {
                if (query.Read())
                {
                    var filename = query["filename"] as string;
                    var data = query["image_data"] as byte[];

                    if (imageFileName == filename && data?.Length > 0)
                    {
                        imageData = data;
                        return true;
                    }
                }
            }

            imageData = null;
            return false;
        }

        private void WriteCache(IDictionary<string, object> @params)
        {
            var connection = this._dbConnection;

            if (connection?.IsConnected ?? false)
            {
                connection.ExecuteNonQuery(Database.QueryCollection.InsertOrReplaceProfileImageCache, @params);
            }
        }

        private bool TryDownloadImage(IUserInfo userInfo, out MemoryStream imageStream)
        {
            try
            {
                var request = WebRequest.CreateHttp(userInfo.ProfileImageUrl);

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var dataStream = new MemoryStream();
                    stream.CopyTo(dataStream);

                    ProfileImageCache.ShrinkImageData(ref dataStream);

                    imageStream = dataStream;
                }

                return true;
            }
            catch
            {
                imageStream = null;
                return false;
            }
        }

        private BitmapImage LoadImage(IUserInfo userInfo)
        {
            var imageUrl = userInfo.ProfileImageUrl;

            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            var imageFileName = Path.GetFileName(imageUrl);

            var @params = CreateParameter(userInfo);
            if (this.TryFindCache(@params, imageFileName, out var imageData))
            {
                return ImageUtility.BitmapSourceFromStream(new MemoryStream(imageData));
            }

            if (this.TryDownloadImage(userInfo, out var imageStream))
            {
                @params.Add("filename", imageFileName);
                @params.Add("image_data", imageStream.ToArray());

                this.WriteCache(@params);

                return ImageUtility.BitmapSourceFromStream(imageStream);
            }

            @params.Clear();
            @params = null;

            return null;
        }

        private async void SetImage(ImageCacheInfo cacheInfo, IUserInfo userInfo)
        {
            cacheInfo.Image = await Task.Run(() => this.LoadImage(userInfo));
        }

        private ImageCacheInfo CreateCache(IUserInfo userInfo)
        {
            var cache = new ImageCacheInfo
            {
                FileName = Path.GetFileName(userInfo.ProfileImageUrl),
            };

            this.SetImage(cache, userInfo);

            return cache;
        }

        private WeakReference<ImageCacheInfo> CreateCacheReference(IUserInfo userInfo)
        {
            var cache = this.CreateCache(userInfo);

            return new WeakReference<ImageCacheInfo>(cache);
        }

        private ImageCacheInfo UpdateCache(IUserInfo userInfo, ImageCacheInfo cacheInfo)
        {
            if (cacheInfo.FileName != Path.GetFileName(userInfo.ProfileImageUrl))
            {
                this.SetImage(cacheInfo, userInfo);
            }

            return cacheInfo;
        }

        private WeakReference<ImageCacheInfo> UpdateCacheReference(IUserInfo userInfo, WeakReference<ImageCacheInfo> reference)
        {
            if (reference.TryGetTarget(out ImageCacheInfo cacheInfo))
            {
                this.UpdateCache(userInfo, cacheInfo);
            }
            else
            {
                reference.SetTarget(this.CreateCache(userInfo));
            }

            return reference;
        }
    }
}
