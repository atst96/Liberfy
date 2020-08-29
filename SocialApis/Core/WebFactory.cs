// #define ALLOW_INVALID_SSL_CERT

using System.Net.Http;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;

namespace SocialApis.Core
{
    /// <summary>
    /// </summary>
    internal static class WebFactory
    {
#if ALLOW_INVALID_SSL_CERT
        /// <summary>
        /// <see cref="HttpClient"/>を生成する。
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateHttpClient()
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = ValidateServerCertificateSV;
            return new HttpClient(clientHandler);
        }
        private static bool ValidateServerCertificateSV(HttpRequestMessage arg1, X509Certificate2 arg2, X509Chain arg3, SslPolicyErrors arg4) => true;

        /// <summary>
        /// <see cref="ClientWebSocket"/>を生成する。
        /// </summary>
        /// <returns></returns>
        public static ClientWebSocket CreateWebSocketClient()
        {
            var webSocket = new ClientWebSocket();
            webSocket.Options.RemoteCertificateValidationCallback = ValidateServerCertificateWS;

            return webSocket;
        }

        private static bool ValidateServerCertificateWS(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
#else
        /// <summary>
        /// <see cref="HttpClient"/>を生成する。
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateHttpClient() => new HttpClient();

        /// <summary>
        /// <see cref="ClientWebSocket"/>を生成する。
        /// </summary>
        /// <returns></returns>
        public static ClientWebSocket CreateWebSocketClient() => new ClientWebSocket();
#endif
    }
}
