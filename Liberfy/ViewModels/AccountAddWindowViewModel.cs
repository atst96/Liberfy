using Liberfy.ViewModels.AccountAddWindow;

namespace Liberfy.ViewModels
{
    /// <summary>
    /// AccountAddWindowのViewModel
    /// </summary>
    internal class AccountAddWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Twitterの認証設定
        /// </summary>
        public TwitterAuthenticationOptionViewModel TwitterOption { get; }

        /// <summary>
        /// Mastodonの認証設定
        /// </summary>
        public MastodonAuthenticationOptionViewModel MastodonOption { get; }

        private IAuthenticationOption _option;
        /// <summary
        /// </summary>
        public IAuthenticationOption Option
        {
            get => this._option;
            set => this.RaisePropertyChangedIfSet(ref this._option, value);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AccountAddWindowViewModel()
        {
            this.TwitterOption = new();
            this.MastodonOption = new();
        }
    }
}
