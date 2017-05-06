using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class StatusReaction : NotificationObject
	{
		public void SetAll(bool isFavorited, bool isRetweeted)
		{
			this.IsFavorited = isFavorited;
			this.IsRetweeted = isRetweeted;
		}

		private bool _isFavorited;
		public bool IsFavorited
		{
			get { return _isFavorited; }
			set { SetProperty(ref _isFavorited, value); }
		}

		private bool _isRetweeted;
		public bool IsRetweeted
		{
			get { return _isRetweeted; }
			set { SetProperty(ref _isRetweeted, value); }
		}
	}
}
