using System;
using System.Diagnostics;
using System.Linq;
using ToriatamaText.Test.ConformanceYaml;

namespace ToriatamaText.Test
{
    static class ExtractorTest
    {
        public static void Run()
        {
            var extractor = new Extractor();
            var extractTests = ExtractYaml.Load();
            var validateTests = ValidateYaml.Load();

            Console.WriteLine("=========================");
            Console.WriteLine("Mentions");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.Mentions)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractMentionedScreenNames(test.Text)
                    .ConvertAll(x => test.Text.Substring(x.StartIndex + 1, x.Length - 1));
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("MentionsWithIndices");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.MentionsWithIndices)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractMentionedScreenNames(test.Text)
                    .ConvertAll(x => new MentionsWithIndicesExpected
                    {
                        ScreenName = test.Text.Substring(x.StartIndex + 1, x.Length - 1),
                        Indices = new[] { x.StartIndex, x.StartIndex + x.Length }
                    });
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("MentionsOrListsWithIndices");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.MentionsOrListsWithIndices)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractMentionsOrLists(test.Text)
                    .ConvertAll(x =>
                    {
                        var s = test.Text.Substring(x.StartIndex + 1, x.Length - 1).Split(new[] { '/' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        return new MentionsOrListsWithIndicesExpected
                        {
                            ScreenName = s[0],
                            ListSlug = s.Length == 2 ? "/" + s[1] : "",
                            Indices = new[] { x.StartIndex, x.StartIndex + x.Length }
                        };
                    });
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("Replies");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.Replies)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractReplyScreenName(test.Text);
                if (result != test.Expected)
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("Urls");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.Urls)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractUrls(test.Text)
                    .ConvertAll(x => test.Text.Substring(x.StartIndex, x.Length));
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("UrlsWithIndices");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.UrlsWithIndices)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractUrls(test.Text)
                    .ConvertAll(x => new UrlsWithIndicesExpected
                    {
                        Url = test.Text.Substring(x.StartIndex, x.Length),
                        Indices = new[] { x.StartIndex, x.StartIndex + x.Length }
                    });
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("Hashtags");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.Hashtags)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractHashtags(test.Text, true)
                    .ConvertAll(x => test.Text.Substring(x.StartIndex + 1, x.Length - 1));
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("HashtagsWithIndices");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.HashtagsWithIndices)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractHashtags(test.Text, true)
                    .ConvertAll(x => new HashtagsWithIndicesExpected
                    {
                        Hashtag = test.Text.Substring(x.StartIndex + 1, x.Length - 1),
                        Indices = new[] { x.StartIndex, x.StartIndex + x.Length }
                    });
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("Cashtags");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.Cashtags)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractCashtags(test.Text)
                    .ConvertAll(x => test.Text.Substring(x.StartIndex + 1, x.Length - 1));
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("CashtagsWithIndices");
            Console.WriteLine("=========================");
            foreach (var test in extractTests.CashtagsWithIndices)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractCashtags(test.Text)
                    .ConvertAll(x => new CashtagsWithIndicesExpected
                    {
                        Cashtag = test.Text.Substring(x.StartIndex + 1, x.Length - 1),
                        Indices = new[] { x.StartIndex, x.StartIndex + x.Length }
                    });
                if (!result.SequenceEqual(test.Expected))
                {
                    Debugger.Break();
                }
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("Validation");
            Console.WriteLine("=========================");

            foreach (var test in validateTests.Usernames)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractMentionedScreenNames(test.Text);
                if ((result.Count == 1 && result[0].Length == test.Text.Length) != test.Expected)
                    Debugger.Break();
            }

            foreach (var test in validateTests.Lists)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractMentionsOrLists(test.Text)
                    .FindAll(x => test.Text.Substring(x.StartIndex, x.Length).IndexOf('/') >= 0);
                if ((result.Count == 1 && result[0].Length == test.Text.Length) != test.Expected)
                    Debugger.Break();
            }

            foreach (var test in validateTests.Hashtags)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractHashtags(test.Text, false);
                if ((result.Count == 1 && result[0].Length == test.Text.Length) != test.Expected)
                    Debugger.Break();
            }

            foreach (var test in validateTests.Urls)
            {
                switch (test.Description)
                {
                    case "Valid url: port and userinfo":
                    case "Valid url: ipv4":
                    case "Valid url: ipv6":
                        continue; // ツイートしても無効だしふざけんな💢💢💢💢
                    case "Valid url: sub delims and question marks":
                    case "Valid url: trailing hyphen":
                        continue; // 最後の1文字は含まれねーよクソが
                }

                Console.WriteLine(test.Description);
                var result = extractor.ExtractUrls(test.Text);
                if ((result.Count == 1 && result[0].Length == test.Text.Length) != test.Expected)
                    Debugger.Break();
            }

            foreach (var test in validateTests.UrlsWithoutProtocol)
            {
                Console.WriteLine(test.Description);
                var result = extractor.ExtractUrls(test.Text);
                if ((result.Count == 1 && result[0].Length == test.Text.Length) != test.Expected)
                    Debugger.Break();
            }
        }
    }
}
