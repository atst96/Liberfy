using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModels;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonValidator : IValidator
    {
        private readonly MastodonAccount _account;

        public MastodonValidator(MastodonAccount account)
        {
            this._account = account;
        }

        public MastodonValidator(int maxTextLength)
        {
            this.MaxPostTextLength = maxTextLength;
        }

        public int MaxPostTextLength { get; }

        public bool CanFavorite(StatusItem item) => true;

        public bool CanPost(ServicePostParameters parameters)
        {
            return parameters.Text?.Length > 0 || (parameters.Attachments.HasItems && parameters.Attachments.Count <= 4);
        }

        public bool CanRetweet(StatusItem item)
        {
            // TODO
            return item.Account.Equals(this._account)
                || item.Status.Visibility == StatusVisibility.Public
                || item.Status.Visibility == StatusVisibility.Unlisted;
        }

        public int GetTextLength(ServicePostParameters parameters)
        {
            int textLength = parameters.Text?.Length ?? 0;

            if (parameters.HasSpoilerText && parameters.SpoilerText != null)
            {
                textLength += parameters.SpoilerText.Length;
            }

            return textLength;
        }
    }
}
