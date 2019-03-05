using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class ServicePostParameters : NotificationObject
    {
        public ServicePostParameters()
        {
            this.Clear();
        }

        private bool _hasSpoilerText;
        public bool HasSpoilerText
        {
            get => this._hasSpoilerText;
            set => this.SetProperty(ref this._hasSpoilerText, value);
        }

        private string _spoilerText;
        public string SpoilerText
        {
            get => this._spoilerText;
            set => this.SetProperty(ref this._spoilerText, NormalizeString(value));
        }

        private string _text;
        public string Text
        {
            get => this._text;
            set => this.SetProperty(ref this._text, NormalizeString(value));
        }

        private bool _isContainsWarningAttachment;
        public bool IsContainsWarningAttachment
        {
            get => this._isContainsWarningAttachment;
            set => this.SetProperty(ref this._isContainsWarningAttachment, value);
        }

        public NotifiableCollection<UploadMedia> Attachments { get; } = new NotifiableCollection<UploadMedia>();

        public void Clear()
        {
            this.SpoilerText = string.Empty;
            this.Text = string.Empty;
            this.Attachments.DisposeAll();
            this.Attachments.Clear();
            this.IsContainsWarningAttachment = App.Setting.NoticePostSensitiveMedia;
        }

        private static string NormalizeString(string value)
        {
            return value == null
                ? string.Empty
                : value.Replace("\r\n", "\n");
        }
    }
}
