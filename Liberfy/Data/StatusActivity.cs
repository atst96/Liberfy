using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class StatusActivity : NotificationObject
    {
        public void Set(bool? isFavorited = null, bool? isRetweeted = null)
        {
            if (isFavorited.HasValue)
            {
                this.IsFavorited = isFavorited.Value;
            }

            if (isRetweeted.HasValue)
            {
                this.IsRetweeted = isRetweeted.Value;
            }
        }

        private bool _isFavorited;
        public bool IsFavorited
        {
            get => this._isFavorited;
            private set => this.SetProperty(ref this._isFavorited, value);
        }

        private bool _isRetweeted;
        public bool IsRetweeted
        {
            get => this._isRetweeted;
            private set => this.SetProperty(ref this._isRetweeted, value);
        }
    }
}
