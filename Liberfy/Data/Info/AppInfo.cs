using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Liberfy
{
	class AppInfo : NotificationObject
	{
		private FontFamily _timelineFont;
		public FontFamily TimelineFont
		{
			get { return _timelineFont; }
			set { SetProperty(ref _timelineFont, value); }
		}
	}
}
