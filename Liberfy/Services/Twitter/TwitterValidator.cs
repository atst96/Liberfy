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

        public int GetTextLength(ServicePostParameters parameters)
        {
            return _tweetTextValidator.GetTweetLength(parameters.Text);
        }

        public bool CanPost(ServicePostParameters parameters)
        {
            int length = GetTextLength(parameters);

            if (parameters.Attachments.Count > 0)
            {
                length += _tweetTextValidator.ShortUrlLengthHttps;
            }

            return length > 0 && length <= this.MaxPostTextLength && parameters.Attachments.Count <= 4;
        }
    }
}
