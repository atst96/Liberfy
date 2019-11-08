using System;
using System.IO;
using System.Net;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ToriatamaText.Test.ConformanceYaml
{
    class ValidateTests
    {
        public TestItem<bool>[] Tweets { get; set; }
        public TestItem<bool>[] Usernames { get; set; }
        public TestItem<bool>[] Lists { get; set; }
        public TestItem<bool>[] Hashtags { get; set; }
        public TestItem<bool>[] Urls { get; set; }
        public TestItem<bool>[] UrlsWithoutProtocol { get; set; }
        public TestItem<int>[] Lengths { get; set; }
    }

    static class ValidateYaml
    {
        private const string testFile = "validate.yml";

        public static ValidateTests Load()
        {
            if (!File.Exists(testFile))
            {
                Console.WriteLine("Downloading validate.yml");
                new WebClient().DownloadFile(
                    "https://github.com/twitter/twitter-text/raw/master/conformance/validate.yml",
                    testFile);
            }

            using (var sr = new StreamReader(testFile))
            {
                var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention(), ignoreUnmatched: true);
                return deserializer.Deserialize<YamlRoot<ValidateTests>>(sr).Tests;
            }
        }
    }
}
