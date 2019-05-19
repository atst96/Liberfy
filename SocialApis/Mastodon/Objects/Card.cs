using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Card
    {
        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "image")]
        public string Image { get; private set; }

        /// <summary>
        /// [string] SocialApis.Mastodon.CardTypes.*
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; private set; }

        [DataMember(Name = "author_name")]
        public string AuthorName { get; private set; }

        [DataMember(Name = "author_url")]
        public string AuthorUrl { get; private set; }

        [DataMember(Name = "provider_name")]
        public string ProviderName { get; private set; }

        [DataMember(Name = "provider_url")]
        public string ProviderUrl { get; private set; }

        [DataMember(Name = "html")]
        public string Html { get; private set; }

        [DataMember(Name = "width")]
        public int Width { get; private set; }

        [DataMember(Name = "height")]
        public int Height { get; private set; }
    }
}
