using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    internal static class Extensions
    {
        public const int DefaultStreamCopyBufferSize = 128 * 1024;

        public static void UploadCopyTo(this Stream source, Stream destination, IProgress<UploadProgress> progress)
        {
            source.UploadCopyTo(destination, progress, DefaultStreamCopyBufferSize);
        }

        public static void UploadCopyTo(this Stream source, Stream destination, IProgress<UploadProgress> progress, int bufferSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            int readByees = 0;
            int sentBytes = 0;
            int totalBytes = (int)source.Length;
            byte[] buffer = new byte[bufferSize];

            while ((readByees = source.Read(buffer, 0, bufferSize)) != 0)
            {
                destination.Write(buffer, 0, readByees);

                sentBytes += readByees;

                progress?.Report(new UploadProgress(sentBytes, totalBytes));
            }
        }

        public static Task UploadCopyToAsync(this Stream source, Stream destination, IProgress<UploadProgress> progress)
        {
            return source.UploadCopyToAsync(destination, progress, DefaultStreamCopyBufferSize);
        }

        public static async Task UploadCopyToAsync(this Stream source, Stream destination, IProgress<UploadProgress> progress, int bufferSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            int readByees = 0;
            int sentBytes = 0;
            int totalBytes = (int)source.Length;
            byte[] buffer = new byte[bufferSize];

            while ((readByees = await source.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, readByees).ConfigureAwait(false);

                sentBytes += readByees;

                progress?.Report(new UploadProgress(sentBytes, totalBytes));
            }
        }
    }
}
