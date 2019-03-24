using Liberfy.Model;
using Liberfy.ViewModels;
using SocialApis.Common;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class MediaEntityInfo
    {
        public IAccount Account { get; }

        public StatusItem Item { get; }

        public Attachment[] Entities { get; }

        public Attachment CurrentAttachment { get; }

        public MediaEntityInfo(IAccount account, StatusItem item)
        {
            this.Account = account;
            this.Item = item;
            this.Entities = this.Item.Status.Attachments;
            this.CurrentAttachment = this.Entities.First();
        }

        public MediaEntityInfo(IAccount account, StatusItem item, Attachment attachment)
        {
            this.Account = account;
            this.Item = item;
            this.Entities = this.Item.Status.Attachments;
            this.CurrentAttachment = attachment;
        }
    }
}
