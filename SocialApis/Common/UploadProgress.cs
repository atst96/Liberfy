using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    public class UploadProgress
    {
        public int UploadedSize { get; internal set; }
        public int TotalSize { get; internal set; }
        public double UploadPercentage { get; internal set; }

        public UploadProgress()
        {
        }

        public UploadProgress(int sentSize, int totalSize)
        {
            this.UploadedSize = sentSize;
            this.TotalSize = totalSize;
            this.UploadPercentage = ((double)sentSize) / totalSize;
        }
    }
}
