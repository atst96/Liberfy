using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static CoreTweet.OAuth;

namespace Liberfy.ViewModel
{
	class AuthenticationViewModel : ContentWindowViewModel
	{
		public AuthenticationViewModel() : base()
		{
			Title = "認証";
		}

		private string _consumerKey;
		public string ConsumerKey
		{
			get { return _consumerKey; }
			set { SetProperty(ref _consumerKey, value, _nextCommand); }
		}

		private string _consumerSecret;
		public string ConsumerSecret
		{
			get { return _consumerSecret; }
			set { SetProperty(ref _consumerSecret, value, _nextCommand); }
		}

		private bool _overrideKey;
		public bool OverrideKey
		{
			get { return _overrideKey; }
			set { SetProperty(ref _overrideKey, value, _nextCommand); }
		}

		private string _error;
		public string Error
		{
			get { return _error; }
			set
			{
				if (SetProperty(ref _error, value))
				{
					HasError = !string.IsNullOrEmpty(value);
					RaisePropertyChanged(nameof(HasError));
				}
			}
		}

		public bool HasError { get; private set; } = false;

		private string _pinCode;
		public string PinCode
		{
			get { return _pinCode; }
			set { SetProperty(ref _pinCode, value, _nextCommand); }
		}

		// page-0: CK/CSの入力
		// page-1: PINコード入力
		// page-2: 
		private int _pageIndex;
		public int PageIndex
		{
			get { return _pageIndex; }
			set { SetProperty(ref _pageIndex, value, _nextCommand); }
		}

		private bool _isRunning;
		public bool IsRunning
		{
			get { return _isRunning; }
			set { SetProperty(ref _isRunning, value, _nextCommand); }
		}

		public OAuthSession Session { get; private set; }
		public Tokens Tokens { get; private set; }

		private CancellationTokenSource _tokenSource;

		private Command _nextCommand;
		public Command NextCommand => _nextCommand
			?? (_nextCommand = new DelegateCommand(async () =>
			{
				Error = string.Empty;

				if (_pageIndex == 0)
				{
					string cKey, cSec;

					if (_overrideKey)
					{
						cKey = ConsumerKey;
						cSec = ConsumerSecret;
					}
					else
					{
						cKey = Defines.ConsumerKey;
						cSec = Defines.ConsumerSecret;
					}

					try
					{
						_tokenSource = new CancellationTokenSource();

						IsRunning = true;

						Session = await AuthorizeAsync(cKey, cSec, cancellationToken: _tokenSource.Token);

						App.Open(Session.AuthorizeUri);
						PageIndex++;
					}
					catch (Exception ex)
					{
						Error = ex.Message;
					}
					finally
					{
						IsRunning = false;
					}
				}
				else if (_pageIndex == 1)
				{
					try
					{
						_tokenSource = new CancellationTokenSource();

						IsRunning = true;

						Tokens = await Session.GetTokensAsync(_pinCode, _tokenSource.Token);

						DialogService.Close(true);
					}
					catch (Exception ex)
					{
						Error = ex.Message;
						PageIndex++;
					}
					finally
					{
						IsRunning = false;
					}
				}

			}, _ =>
			{
				if (IsRunning)
					return false;

				switch (PageIndex)
				{
					case 0: return !(_overrideKey && (string.IsNullOrWhiteSpace(ConsumerKey) || string.IsNullOrWhiteSpace(ConsumerSecret)));
					case 1: return _pinCode?.Length == 7 && Regex.IsMatch(_pinCode, @"^\d+$");
					default: return false;
				}
			}));

		private Command _cancelCommand;
		public Command CancelCommand => _cancelCommand
			?? (_cancelCommand = new DelegateCommand(() =>
			{
				if (_tokenSource != null)
				{
					_tokenSource.Cancel();
				}

				DialogService.Close(false);
			}));

		private Command _copyClipboardCommand;
		public Command CopyClipboardCommand => _copyClipboardCommand
			?? (_copyClipboardCommand = new DelegateCommand(() =>
			{
				if (Session?.AuthorizeUri != null)
				{
					System.Windows.Clipboard.SetText(Session.AuthorizeUri.AbsoluteUri);
				}
			}));
	}
}
