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
		internal override async void OnInitialized()
		{
			if (_initialized) return;
			_initialized = true;

			if (Accounts.Count == 0)
			{
				if (!DialogService.OpenSetting(1, true))
				{
					DialogService.Invoke(ViewState.Close);
					return;
				}
			}

			await Task.WhenAll(
				Accounts.Select(a => Task.Run(() =>
				{
					if (true)
					{
						a.IsLoading = true;

						a.Timeline.LoadAccount();

						a.IsLoading = false;
					}
				})));
		}

		public ClientContent Client { get; } = App.Client;

		private Command _showSettingDialog;
		public Command ShowSettingDialog => _showSettingDialog
			?? (_showSettingDialog = new DelegateCommand(() =>
			{
				DialogService.OpenSetting();
			}));

		public override bool CanClose()
		{
			App.Shutdown(true);

			return true;
		}
	}
}
