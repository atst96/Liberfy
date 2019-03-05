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
        public MastodonValidator(int maxTextLength)
        {
            this.MaxPostTextLength = maxTextLength;
        }

        public int MaxPostTextLength { get; }

        public bool CanPost(ServicePostParameters parameters)
        {
            return parameters.Text?.Length > 0 || (parameters.Attachments.HasItems && parameters.Attachments.Count <= 4);
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
