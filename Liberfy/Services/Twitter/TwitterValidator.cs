using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriatamaText;

namespace Liberfy.Services.Twitter
{
    internal class TwitterValidator : IValidator
    {
        private static readonly Validator _tweetTextValidator = new Validator();
        private readonly TwitterAccount _account;

        public int MaxPostTextLength { get; } = 140;

        public TwitterValidator(TwitterAccount account)
        {
            this._account = account;
        }

        public int GetTextLength(ServicePostParameters parameters)
        {
            return _tweetTextValidator.GetTweetLength(parameters.Text);
        }

        public bool CanPost(ServicePostParameters parameters)
        {
            int length = GetTextLength(parameters);

            if (parameters.Attachments.Count > 0)
            {
                length += _tweetTextValidator.ShortUrlLength;
            }

            return length > 0 && length <= this.MaxPostTextLength && parameters.Attachments.Count <= 4;
        }

        public bool CanFavorite(StatusItem item) => true;

        public bool CanRetweet(StatusItem item)
        {
            return item.Account.Equals(this._account) || !item.User.IsProtected;
        }
    }
}
