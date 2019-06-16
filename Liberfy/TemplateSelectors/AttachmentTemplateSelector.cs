using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Liberfy.Model;

namespace Liberfy.TemplateSelectors
{
    internal class AttachmentTemplateSelector : DataTemplateSelector
    {
        private static readonly DataTemplate _photoAttachmentTemplate = App.Instance.TryFindResource<DataTemplate>("PhotoAttachmentTemplate");
        private static readonly DataTemplate _videoAttachmentTemplate = App.Instance.TryFindResource<DataTemplate>("VideoAttachmentTemplate");

        public AttachmentTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;

            if (!(item is Attachment attachment))
                throw new NotSupportedException();

            switch (attachment.Type)
            {
                case AttachmentType.Photo:
                    return _photoAttachmentTemplate;

                case AttachmentType.Gif:
                case AttachmentType.Video:
                    return _videoAttachmentTemplate;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
