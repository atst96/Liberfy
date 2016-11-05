using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class MainWindowViewModel : ViewModelBase
	{
		private bool _initialized;
		public void Initialize()
		{
			if (_initialized) return;
			_initialized = true;

			DialogService.OpenSetting(1, true);
		}
	}
}
