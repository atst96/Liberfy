namespace SocialApis.Mastodon
{
    public abstract class StreamResponse
    {
        /// <summary>
        /// イベント種別
        /// </summary>
        public StreamEventType EventType { get; }

        protected StreamResponse(StreamEventType eventType)
        {
            this.EventType = eventType;
        }
    }
}
