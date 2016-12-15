using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class MainWindowViewModel : ViewModelBase
	{
		public AccountCollection Accounts { get; } = App.Accounts;

		private bool _initialized;
		internal override void OnInitialized()
		{
			if (_initialized) return;
			_initialized = true;

			if (Accounts.Count == 0)
			{
				if(!DialogService.OpenSetting(1, true))
				{
					DialogService.Invoke(ViewState.Close);
					return;
				}
			}
		}

		public override bool CanClose()
		{
			App.Shutdown(true);

			return true;
		}
	}
}
