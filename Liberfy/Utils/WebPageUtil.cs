using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sgml;

namespace Liberfy.Utils
{
    internal static class WebPageUtil
    {
        private const string DefaultFaviconPath = "./favicon.ico";

        /// <summary>
        /// FaviconのURLを取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<Uri> GetFaviconUrl(Uri url)
        {
            // TODO: HttpClientFactory
            using var client = new HttpClient();
            using var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url))
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var stream = response.Content.ReadAsStream();

            using var sgmlReader = new SgmlReader
            {
                DocType = "html",
                IgnoreDtd = true,
                CaseFolding = CaseFolding.ToLower,
                InputStream = new StreamReader(stream, Encoding.UTF8),
            };

            var rootElement = XElement.Load(sgmlReader, LoadOptions.PreserveWhitespace);
            var headElement = rootElement.Element("head");
            var linkElement = headElement.Elements()
                .FirstOrDefault(e => e.Name == "link" && (e.Attribute("rel")?.Value?.ToLower().Split(" ").Contains("icon") ?? false));
            var favicon = linkElement?.Attribute("href")?.Value;

            return !string.IsNullOrWhiteSpace(favicon)
                ? new Uri(url, favicon)
                : new Uri(url, DefaultFaviconPath);
        }
    }
}
