namespace SocialApis.Mastodon
{
    public class StreamNotificationResponse : StreamResponse
    {
        /// <summary>
        /// 通知内容
        /// </summary>
        public Notification Notification { get; }

        internal StreamNotificationResponse(Notification notification)
            : base(StreamEventType.Notification)
        {
            this.Notification = notification;
        }
    }
}
