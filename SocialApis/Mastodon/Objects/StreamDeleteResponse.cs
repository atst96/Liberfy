namespace SocialApis.Mastodon
{
    public class StreamDeleteResponse : StreamResponse
    {
        /// <summary>
        /// StatusID
        /// </summary>
        public long StatusId { get; }

        internal StreamDeleteResponse(long statusId)
            : base(StreamEventType.Delete)
        {
            this.StatusId = statusId;
        }
    }
}
