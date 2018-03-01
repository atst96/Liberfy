using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Twitter
{
    internal static class StreamJsonFormatters
    {
        public static IJsonFormatterResolver Resolver { get; } = Utf8Json.Resolvers.StandardResolver.AllowPrivate;

        private static IJsonFormatter<DeleteStreamResponse> _delete;
        public static IJsonFormatter<DeleteStreamResponse> Delete => _delete ?? (_delete = Resolver.GetFormatter<DeleteStreamResponse>());

        private static IJsonFormatter<ScrubGeoStreamResponse> _scrubGeo;
        public static IJsonFormatter<ScrubGeoStreamResponse> ScrubGeo => _scrubGeo ?? (_scrubGeo = Resolver.GetFormatter<ScrubGeoStreamResponse>());

        private static IJsonFormatter<StatusWithheldStreamResponse> _statusWithheld;
        public static IJsonFormatter<StatusWithheldStreamResponse> StatusWithheld => _statusWithheld ?? (_statusWithheld = Resolver.GetFormatter<StatusWithheldStreamResponse>());

        private static IJsonFormatter<UserWithheldStreamResponse> _userWithheld;
        public static IJsonFormatter<UserWithheldStreamResponse> UserWithheld => _userWithheld ?? (_userWithheld = Resolver.GetFormatter<UserWithheldStreamResponse>());

        private static IJsonFormatter<DisconnectStreamResponse> _disconnect;
        public static IJsonFormatter<DisconnectStreamResponse> Disconnect => _disconnect ?? (_disconnect = Resolver.GetFormatter<DisconnectStreamResponse>());

        private static IJsonFormatter<WarningStreamResponse> _warning;
        public static IJsonFormatter<WarningStreamResponse> Warning => _warning ?? (_warning = Resolver.GetFormatter<WarningStreamResponse>());

        private static IJsonFormatter<ControlStreamResponse> _control;
        public static IJsonFormatter<ControlStreamResponse> Control => _control ?? (_control = Resolver.GetFormatter<ControlStreamResponse>());

        private static IJsonFormatter<DirectMessageStreamResponse> _directMessage;
        public static IJsonFormatter<DirectMessageStreamResponse> DirectMessage => _directMessage ?? (_directMessage = Resolver.GetFormatter<DirectMessageStreamResponse>());

        private static IJsonFormatter<Status> _statusObject;
        public static IJsonFormatter<Status> StatusObject => _statusObject ?? (_statusObject = Resolver.GetFormatter<Status>());

        private static IJsonFormatter<List> _listObject;
        public static IJsonFormatter<List> ListObject => _listObject ?? (_listObject = Resolver.GetFormatter<List>());
    }
}
