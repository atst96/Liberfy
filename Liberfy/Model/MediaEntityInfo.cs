using Liberfy.Model;
using Liberfy.ViewModel;
using SocialApis.Common;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    struct MediaEntityInfo
    {
        public AccountBase Account { get; }

        public StatusItem Item { get; }

        public Attachment[] Entities { get; }

        public Attachment CurrentEntity { get; }

        public MediaEntityInfo(AccountBase account, StatusItem item)
        {
            this.Account = account;
            this.Item = item;
            this.Entities = this.Item.Status.Attachments;
            this.CurrentEntity = this.Entities.First();
        }

        public MediaEntityInfo(AccountBase account, StatusItem item, Attachment currentEntity)
        {
            this.Account = account;
            this.Item = item;
            this.Entities = this.Item.Status.Attachments;
            this.CurrentEntity = currentEntity;
        }
    }
}
