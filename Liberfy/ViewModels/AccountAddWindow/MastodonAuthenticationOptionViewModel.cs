using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Liberfy.Data.Mastodon;
using Liberfy.Utils;
using SocialApis.Mastodon;

namespace Liberfy.ViewModels.AccountAddWindow
{
    /// <summary>
    /// Mastodonの認証オプション
    /// </summary>
    internal class MastodonAuthenticationOptionViewModel : NotificationObject, IAuthenticationOption
    {
        public string ServiceName { get; } = "Mastodon";

        /// <summary>
        /// インスタンス名の正規表現
        /// </summary>
        private Regex _instanceRegex = new("^[a-zA-Z0-9][a-zA-Z0-9\\-\\.]+[a-zA-Z]+$", RegexOptions.Compiled);

        private bool _isInstanceLoading;

        /// <summary>
        /// インスタンス情報の読み込み中フラグ
        /// </summary>5
        public bool IsInstanceLoading
        {
            get => this._isInstanceLoading;
            private set => this.SetProperty(ref this._isInstanceLoading, value);
        }

        private string _loadInstanceErrorMessage;

        /// <summary>
        /// インスタンス読み込みエラー
        /// </summary>
        public string LoadInstanceErrorMessage
        {
            get => this._loadInstanceErrorMessage;
            private set => this.SetProperty(ref this._loadInstanceErrorMessage, value);
        }

        private bool _isLoadInstanceFailed;

        /// <summary>
        /// インスタンスの読み込みが失敗のフラグ
        /// </summary>
        public bool IsLoadInstanceFailed
        {
            get => this._isLoadInstanceFailed;
            private set => this.SetProperty(ref this._isLoadInstanceFailed, value);
        }

        private string _instanceName;

        /// <summary>
        /// インスタンス
        /// </summary>
        public string InstanceName
        {
            get => this._instanceName;
            set
            {
                if (!this.SetProperty(ref this._instanceName, value))
                {
                    return;
                }

                _ = this.UpdateInstanceInfo(value);
            }
        }

        /// <summary>
        /// インスタンス情報取得時のエラーを設定する
        /// </summary>
        /// <param name="message"></param>
        private void SetInstanceError(string message)
        {
            this.LoadInstanceErrorMessage = message;
            this.IsLoadInstanceFailed = true;
        }

        /// <summary>
        /// インスタンス情報のエラーをクリアする
        /// </summary>
        private void ClearInstanceError()
        {
            this.LoadInstanceErrorMessage = default;
            this.IsLoadInstanceFailed = false;
        }

        /// <summary>
        /// インスタンス情報を更新する
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        private async Task UpdateInstanceInfo(string instanceName)
        {
            this.IsInstanceLoading = true;
            this.Instance = default;
            this.UpdateIsInvalid();
            this.ClearInstanceError();

            try
            {
                if (string.IsNullOrEmpty(instanceName))
                {
                    this.SetInstanceError("インスタンス名を入力してください");
                    return;
                }

                if (!this._instanceRegex.IsMatch(instanceName))
                {
                    this.SetInstanceError("無効なインスタンス名です");
                    return;
                }

                if (!Uri.TryCreate($"https://{instanceName}", UriKind.Absolute, out var url))
                {
                    this.SetInstanceError("無効なインスタンス名です");
                    return;
                }

                var api = new MastodonApi(url, null, null);

                Instance instance;
                try
                {
                    instance = await api.Instances.GetInstance()
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException)
                {
                    this.SetInstanceError("インスタンスへの接続に失敗しました");
                    return;
                }

                // Faviconを取得する
                var favicon = await WebPageUtil.GetFaviconUrl(url)
                    .ConfigureAwait(false);

                this.Instance = new InstanceInfo
                {
                    Title = instance.Title,
                    Description = instance.Description,
                    Icon = favicon,
                    Url = url,
                    StreamingApiEndpoint = instance.Urls?.StreamingApi,
                    Version = instance.Version,
                    Languages = instance.Languages,
                };
            }
            finally
            {
                this.IsInstanceLoading = false;
                this.UpdateIsInvalid();
            }
        }

        private InstanceInfo _instance;

        /// <summary>
        /// インスタンス情報
        /// </summary>
        public InstanceInfo Instance
        {
            get => this._instance;
            private set
            {
                if (this.SetProperty(ref this._instance, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasInstance));
                }
            }
        }

        public bool HasInstance => this.Instance != default;

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

        private string _consumerSecret;

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

        private string _consumerKey;

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
        /// 設定値が有効かどうかを更新する
        /// </summary>
        private void UpdateIsInvalid()
        {
            this.IsInvalid = this.Instance != null
                && (!this.OverrideKeys || (!string.IsNullOrEmpty(this.ConsumerKey) && !string.IsNullOrEmpty(this.ConsumerSecret)));
        }

        private bool _isInvalid;

        /// <summary>
        /// 設定値が有効かどうかを取得する
        /// </summary>
        public bool IsInvalid
        {
            get => this._isInvalid;
            private set => this.SetProperty(ref this._isInvalid, value);
        }
    }
}
