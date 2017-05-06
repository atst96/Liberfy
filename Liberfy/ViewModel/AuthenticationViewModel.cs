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
			get => _consumerKey;
			set => SetProperty(ref _consumerKey, value, _nextCommand);
		}

		private string _consumerSecret;
		public string ConsumerSecret
		{
			get => _consumerSecret;
			set => SetProperty(ref _consumerSecret, value, _nextCommand);
		}

		private bool _overrideKey;
		public bool OverrideKey
		{
			get => _overrideKey;
			set => SetProperty(ref _overrideKey, value, _nextCommand);
		}

		private string _error;
		public string Error
		{
			get => _error;
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
			get => _pinCode;
			set => SetProperty(ref _pinCode, value, _nextCommand);
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

		#region Command: NextCommand

		private Command _nextCommand;
		public Command NextCommand
		{
			get => _nextCommand ?? (_nextCommand = RegisterReleasableCommand(MoveNextPage, CanMoveNextPage));
		}

		private async void MoveNextPage()
		{
			Error = string.Empty;

			if (_pageIndex == 0)
			{
				// page-0: 認証URLの取得

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
				// page-1: PINコードを用いて認証

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
		}

		private bool CanMoveNextPage(object p)
		{
			if (IsRunning)
				return false;

			switch (_pageIndex)
			{
				case 0:
					if (_overrideKey)
					{
						bool isInvalidConsumerKey = !string.IsNullOrWhiteSpace(ConsumerKey);
						bool isInvalidConsumerSecret = !string.IsNullOrWhiteSpace(ConsumerSecret);

						return isInvalidConsumerKey && isInvalidConsumerSecret;
					}
					else
					{
						return true;
					}

				case 1:
					return _pinCode?.Length == 7
						&& Regex.IsMatch(_pinCode, @"^\d+$");

				default:
					return false;
			}
		}

		#endregion

		#region Command: CancelCommand

		private Command _cancelCommand;
		public Command CancelCommand
		{
			get => _cancelCommand ?? (_cancelCommand = RegisterReleasableCommand(CancelAll));
		}

		private void CancelAll()
		{
			if (_tokenSource != null
				&& !_tokenSource.IsCancellationRequested)
			{
				_tokenSource.Cancel();
			}

			DialogService.Close(false);
		}

		#endregion

		#region Command: CopyClipboardCommand

		private Command _copyClipboardCommand;
		public Command CopyClipboardCommand
		{
			get => _copyClipboardCommand ?? (_copyClipboardCommand = RegisterReleasableCommand(CopyAuthorizeUrlToClipboard));
		}

		private void CopyAuthorizeUrlToClipboard()
		{
			if (Session.AuthorizeUri != null)
			{
				System.Windows.Clipboard.SetText(Session.AuthorizeUri.AbsoluteUri);
			}
		}

		#endregion
	}
}
