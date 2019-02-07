using Liberfy.ViewModel;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands
{
    internal class PostCommand : Command<IAccount>
    {
        private TweetWindow _viewModel;

        public PostCommand(TweetWindow viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(IAccount parameter)
        {
            return parameter != null && this._viewModel.CanPostTweet();
        }

        protected override async void Execute(IAccount account)
        {
            this._viewModel.OnPostBegin();

            var tokens = (TwitterApi)account.Tokens;

            var uploadPrams = new SocialApis.Query
            {
                ["status"] = this._viewModel.Tweet,
                ["possibly_sensitive"] = this._viewModel.IsSensitiveMedia,
            };

            if (this._viewModel.HasReplyStatus)
                uploadPrams["in_reply_to_status_id"] = this._viewModel.ReplyToStatus.Id;


            // 画像および動画のアップロード
            var uploadableMedia = this.GetUploadableMedia(this._viewModel.Media);
            if (uploadableMedia.Any())
            {
                this._viewModel.UploadStatusText = "メディアをアップロードしています...";
                foreach (var mediaItem in uploadableMedia)
                    mediaItem.SetIsTweetPosting(true);

                if (await this.UploadMediaItems(tokens, uploadableMedia).ConfigureAwait(true))
                {
                    uploadPrams["media_ids"] = this._viewModel.Media
                        .Where(m => m.IsAvailableUploadId())
                        .Select(m => m.UploadId.Value);
                }
                else
                {
                    foreach (var mediaItem in uploadableMedia)
                        mediaItem.SetIsTweetPosting(false);

                    this._viewModel.OnPostEnd();

                    return;
                }
            }

            // ツイート
            this._viewModel.UploadStatusText = "ツイートしています...";

            try
            {
                await tokens.Statuses.Update(uploadPrams);

                this.OnPostComplated();
            }
            catch (Exception ex)
            {
                this._viewModel.DialogService.MessageBox(ex.Message, null);
            }

            this._viewModel.OnPostEnd();

            if (this._viewModel.CloseOnPostComplated)
            {
                this._viewModel.DialogService.Invoke(ViewState.Close);
            }
        }

        private async Task<bool> UploadMediaItems(TwitterApi token, IEnumerable<UploadMedia> media)
        {
            await Task.WhenAll(media.Select(m => m.Upload(token)));

            var failedResults = media.Where(m => m.IsUploadFailed);
            int failedsCount = failedResults.Count();

            if (failedsCount > 0)
            {
                if (this._viewModel.DialogService.MessageBox(
                    $"{ media.Count() }件中{ failedsCount }件 アップロードに失敗しました。\n再試行しますか？",
                    MsgBoxButtons.RetryCancel, MsgBoxIcon.Question) == MsgBoxResult.Retry)
                {
                    return await this.UploadMediaItems(token, this.GetUploadableMedia(media));
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<UploadMedia> GetUploadableMedia(IEnumerable<UploadMedia> media)
        {
            return media.Where(m => !m.IsAvailableUploadId());
        }

        private void OnPostComplated()
        {
            this._viewModel.ClearStatus();
        }
    }
}
