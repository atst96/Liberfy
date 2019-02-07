using Liberfy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.Text;

namespace Liberfy.Services
{
    internal class TwitterValidator : IValidator
    {
        private static readonly Validator _tweetTextValidator = new Validator();

        public int MaxPostTextLength { get; } = 140;

        public int CountTextLength(string text)
        {
            return _tweetTextValidator.GetTweetLength(text);
        }

        public bool CanPost(int textLength, ICollection<UploadMedia> sources)
        {
            int length = textLength;

            if (sources.Count > 0)
            {
                length += _tweetTextValidator.ShortUrlLengthHttps;
            }

            return length > 0 && length <= this.MaxPostTextLength;
        }
    }
}
