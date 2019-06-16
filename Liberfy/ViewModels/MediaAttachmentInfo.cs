using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Components;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    internal class MediaAttachmentInfo : NotificationObject
    {
        public StatusItem StatusItem { get; }
        public IReadOnlyList<Model.Attachment> Items { get; }
        public Model.Attachment Item { get; }
        public int Index { get; }

        public MediaAttachmentInfo(StatusItem statusItem, int attachmentIndex)
        {
            this.StatusItem = statusItem;
            this.Index = attachmentIndex;
            this.Items = statusItem.Status.Attachments;
            this.Item = this.Items[attachmentIndex];
        }
    }
}
