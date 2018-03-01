using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class StatusReaction : NotificationObject
    {
        public void SetAll(bool isFavorited, bool isRetweeted)
        {
            this.IsFavorited = isFavorited;
            this.IsRetweeted = isRetweeted;
        }

        private bool _isFavorited;
        public bool IsFavorited
        {
            get => _isFavorited;
            set => SetProperty(ref _isFavorited, value);
        }

        private bool _isRetweeted;
        public bool IsRetweeted
        {
            get => _isRetweeted;
            set => SetProperty(ref _isRetweeted, value);
        }
    }
}
