using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Model;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    internal class MediaPreviewWindowViewModel : ViewModelBase
    {
        private StatusItem _status;
        public StatusItem Status
        {
            get => this._status;
            private set => this.SetProperty(ref this._status, value);
        }

        private bool _isShowNavigator;
        public bool IsShowNavigator
        {
            get => this._isShowNavigator;
            private set => this.SetProperty(ref this._isShowNavigator, value);
        }

        private IReadOnlyList<Attachment> _attachments;
        public IReadOnlyList<Attachment> Attachments
        {
            get => this._attachments;
            private set => this.SetProperty(ref this._attachments, value);
        }

        private int _selectedAttachmentIndex;
        public int SelectedAttachmentIndex
        {
            get => this._selectedAttachmentIndex;
            set
            {
                this.SetProperty(ref this._selectedAttachmentIndex, value);

                this.SelectPreviousAttachmentCommand.RaiseCanExecute();
                this.SelectNextAttachmentCommand.RaiseCanExecute();

            }
        }

        public Command SelectPreviousAttachmentCommand { get; }
        public Command SelectNextAttachmentCommand { get; }

        public MediaPreviewWindowViewModel()
        {
            this.SelectPreviousAttachmentCommand = this.RegisterCommand(new DelegateCommand(this.SelectPreviousAttachment, this.CanSelectPreviouwAttachment));
            this.SelectNextAttachmentCommand = this.RegisterCommand(new DelegateCommand(this.SelectNextAttachment, this.CanSelectNextAttachment));
        }

        public void SetMediaItemInfo(MediaAttachmentInfo mediaItem)
        {
            this.Status = mediaItem.StatusItem;
            this.Attachments = mediaItem.StatusItem.Status.Attachments;
            this.SelectedAttachmentIndex = mediaItem.Index;
            this.IsShowNavigator = this.Attachments.Count > 1;
        }

        public void SelectPreviousAttachment()
        {
            this.SelectedAttachmentIndex -= 1;
        }

        public bool CanSelectPreviouwAttachment()
        {
            return this.Attachments != null
                && this.SelectedAttachmentIndex >= 1;
        }

        public void SelectNextAttachment()
        {
            this.SelectedAttachmentIndex += 1;
        }

        public bool CanSelectNextAttachment()
        {
            return this.Attachments != null
                && this.SelectedAttachmentIndex < this.Attachments.Count - 1;
        }
    }
}
