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
        public void SetAll(bool isFavorited, bool isRetweeted)
        {
            this.IsFavorited = isFavorited;
            this.IsRetweeted = isRetweeted;
        }

        private bool _isFavorited;
        public bool IsFavorited
        {
            get => this._isFavorited;
            set => this.SetProperty(ref this._isFavorited, value);
        }

        private bool _isRetweeted;
        public bool IsRetweeted
        {
            get => this._isRetweeted;
            set => this.SetProperty(ref this._isRetweeted, value);
        }
    }
}
