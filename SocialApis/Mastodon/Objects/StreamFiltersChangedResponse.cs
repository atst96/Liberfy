namespace SocialApis.Mastodon.Objects
{
    public class StreamFiltersChangedResponse : StreamResponse
    {
        internal StreamFiltersChangedResponse()
            : base(StreamEventType.FilterChanged)
        {
        }
    }
}
