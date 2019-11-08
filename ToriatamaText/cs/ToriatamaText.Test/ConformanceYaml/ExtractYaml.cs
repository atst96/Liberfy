using System;
using System.IO;
using System.Linq;
using System.Net;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

#pragma warning disable CS0659 // 型は Object.Equals(object o) をオーバーライドしますが、Object.GetHashCode() をオーバーライドしません

namespace ToriatamaText.Test.ConformanceYaml
{
    class ExtractTests
    {
        public TestItem<string[]>[] Mentions { get; set; }
        public TestItem<MentionsWithIndicesExpected[]>[] MentionsWithIndices { get; set; }
        public TestItem<MentionsOrListsWithIndicesExpected[]>[] MentionsOrListsWithIndices { get; set; }
        public TestItem<string>[] Replies { get; set; }
        public TestItem<string[]>[] Urls { get; set; }
        public TestItem<UrlsWithIndicesExpected[]>[] UrlsWithIndices { get; set; }
        public TestItem<string[]>[] Hashtags { get; set; }
        public TestItem<HashtagsWithIndicesExpected[]>[] HashtagsWithIndices { get; set; }
        public TestItem<string[]>[] Cashtags { get; set; }
        public TestItem<CashtagsWithIndicesExpected[]>[] CashtagsWithIndices { get; set; }
    }

    class MentionsWithIndicesExpected
    {
        public string ScreenName { get; set; }
        public int[] Indices { get; set; }

        public override bool Equals(object obj)
        {
            var x = obj as MentionsWithIndicesExpected;
            if (x == null) return false;
            return this.ScreenName == x.ScreenName && this.Indices.SequenceEqual(x.Indices);
        }
    }

    class MentionsOrListsWithIndicesExpected
    {
        public string ScreenName { get; set; }
        public string ListSlug { get; set; }
        public int[] Indices { get; set; }

        public override bool Equals(object obj)
        {
            var x = obj as MentionsOrListsWithIndicesExpected;
            if (x == null) return false;
            return this.ScreenName == x.ScreenName && this.ListSlug == x.ListSlug && this.Indices.SequenceEqual(x.Indices);
        }
    }

    class UrlsWithIndicesExpected
    {
        public string Url { get; set; }
        public int[] Indices { get; set; }

        public override bool Equals(object obj)
        {
            var x = obj as UrlsWithIndicesExpected;
            if (x == null) return false;
            return this.Url == x.Url && this.Indices.SequenceEqual(x.Indices);
        }
    }

    class HashtagsWithIndicesExpected
    {
        public string Hashtag { get; set; }
        public int[] Indices { get; set; }

        public override bool Equals(object obj)
        {
            var x = obj as HashtagsWithIndicesExpected;
            if (x == null) return false;
            return this.Hashtag == x.Hashtag && this.Indices.SequenceEqual(x.Indices);
        }
    }

    class CashtagsWithIndicesExpected
    {
        public string Cashtag { get; set; }
        public int[] Indices { get; set; }

        public override bool Equals(object obj)
        {
            var x = obj as CashtagsWithIndicesExpected;
            if (x == null) return false;
            return this.Cashtag == x.Cashtag && this.Indices.SequenceEqual(x.Indices);
        }
    }

    static class ExtractYaml
    {
        private const string testFile = "extract.yml";

        public static ExtractTests Load()
        {
            if (!File.Exists(testFile))
            {
                Console.WriteLine("Downloading extract.yml");
                new WebClient().DownloadFile(
                    "https://raw.githubusercontent.com/twitter/twitter-text/master/conformance/extract.yml",
                    testFile);
            }

            using (var sr = new StreamReader(testFile))
            {
                var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention(), ignoreUnmatched: true);
                return deserializer.Deserialize<YamlRoot<ExtractTests>>(sr).Tests;
            }
        }
    }
}
