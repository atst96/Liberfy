namespace Liberfy.ViewModels.AccountAddWindow
{
    /// <summary>
    /// Twitterの認証オプション
    /// </summary>
    internal class TwitterAuthenticationOptionViewModel : NotificationObject, IAuthenticationOption
    {
        public string ServiceName { get; } = "Twitter";

        public TwitterAuthenticationOptionViewModel()
        {
            this.UpdateIsInvalid();
        }

        private bool _overrideKeys;

        /// <summary>
        /// 独自キーを使用する
        /// </summary>
        public bool OverrideKeys
        {
            get => this._overrideKeys;
            set
            {
                if (this.SetProperty(ref this._overrideKeys, value))
                {
                    this.UpdateIsInvalid();
                }
            }
        }

        private string _consumerKey;

        /// <summary>
        /// ConsumerKey
        /// </summary>
        public string ConsumerKey
        {
            get => this._consumerKey;
            set
            {
                if (this.SetProperty(ref this._consumerKey, value))
                {
                    this.UpdateIsInvalid();
                }
            }
        }

        private string _consumerSecret;

        /// <summary>
        /// ConsumerSecret
        /// </summary>
        public string ConsumerSecret
        {
            get => this._consumerSecret;
            set
            {
                if (this.SetProperty(ref this._consumerSecret, value))
                {
                    this.UpdateIsInvalid();
                }
            }
        }

        /// <summary>
        /// 入力値が有効かどうかを更新する
        /// </summary>
        private void UpdateIsInvalid()
        {
            this.IsInvalid = !this.OverrideKeys || (!string.IsNullOrWhiteSpace(this.ConsumerKey) && !string.IsNullOrWhiteSpace(this.ConsumerSecret));
        }

        private bool _isInvalid;
        /// <summary>
        /// 入力値が有効かどうかを取得する
        /// </summary>
        public bool IsInvalid
        {
            get => this._isInvalid;
            private set => this.SetProperty(ref this._isInvalid, value);
        }
    }
}
