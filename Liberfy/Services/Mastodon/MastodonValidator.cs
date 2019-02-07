using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModel;

namespace Liberfy.Services
{
    internal class MastodonValidator : IValidator
    {
        public MastodonValidator(int maxTextLength)
        {
            this.MaxPostTextLength = maxTextLength;
        }

        public int MaxPostTextLength { get; }

        public bool CanPost(int textLength, ICollection<UploadMedia> sources)
        {
            return textLength > 0 || sources.Count > 0;
        }

        public int CountTextLength(string text)
        {
            return text.Length;
        }
    }
}
