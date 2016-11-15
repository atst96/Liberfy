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
		internal override void OnInitialized()
		{
			if (_initialized) return;
			_initialized = true;

			DialogService.OpenSetting(1, true);
		}
	}
}
