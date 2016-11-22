using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
	class SettingWindowViewModel : ViewModelBase
	{
		internal override void OnInitialized()
		{
			base.OnInitialized();
		}

		public override bool CanClose()
		{
			if(!App.Accounts.Any())
			{
				switch(DialogService.MessageBox(
					"アカウントが登録されていません。終了しますか？", "Liberfy",
					MsgBoxButtons.YesNo, MsgBoxIcon.Question))
				{
					case MsgBoxResult.Yes:
						App.ForceExit();
						return true;

					default:
						return false;
				}
			}

			return true;
		}
	}
}
