using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class MainWindowViewModel : ViewModelBase
	{
		private bool hasAccounts => App.Accounts.Count > 0;

		private bool _initialized;
		internal async override void OnInitialized()
		{
			if (_initialized) return;
			_initialized = true;

			if (!hasAccounts)
			{
				DialogService.OpenSetting(1, true);

				if (!hasAccounts)
				{
					DialogService.Invoke(ViewState.Close);
				}
			}
		}
	}
}
