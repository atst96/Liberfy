using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Core
{
    /// <summary>
    /// アップロードの進捗状況
    /// </summary>
    public class UploadReport
    {
        /// <summary>
        /// 状態
        /// </summary>
        public ProgressStatus Status { get; private set; }

        /// <summary>
        /// アップロード済みのバイト数
        /// </summary>
        public long UploadedByteLength { get; private set; }

        /// <summary>
        /// 合計のバイト数
        /// </summary>
        public long TotalLength { get; private set; }

        /// <summary>
        /// アップロードの進捗率
        /// </summary>
        public double UploadPercentage { get; private set; }

        public UploadReport(ProgressStatus status)
        {
            this.Status = status;
        }

        public UploadReport(ProgressStatus status, long sentSize, long totalSize)
        {
            this.Status = status;
            this.UploadedByteLength = sentSize;
            this.TotalLength = totalSize;
            this.UploadPercentage = ((double)sentSize) / totalSize;
        }

        public static UploadReport CreateBegin(long total)
            => new UploadReport(ProgressStatus.Begin, 0, total);

        public static UploadReport CreateProcessing(long actual, long total)
            => new UploadReport(ProgressStatus.Processing, actual, total);

        public static UploadReport CreateCompleted(long total)
            => new UploadReport(ProgressStatus.Completed, total, total);
    }
}
