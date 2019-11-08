using System;
using System.Diagnostics;
using ToriatamaText.Test.ConformanceYaml;

namespace ToriatamaText.Test
{
    static class ValidatorTest
    {
        public static void Run()
        {
            var validator = new Validator();
            var tests = ValidateYaml.Load();

            Console.WriteLine("=========================");
            Console.WriteLine("Tweets");
            Console.WriteLine("=========================");
            foreach (var test in tests.Tweets)
            {
                Console.WriteLine(test.Description);
                var result = validator.IsValidTweet(test.Text);
                if (result != test.Expected)
                    Debugger.Break();
            }

            Console.WriteLine();
            Console.WriteLine("=========================");
            Console.WriteLine("Lengths");
            Console.WriteLine("=========================");
            foreach (var test in tests.Lengths)
            {
                Console.WriteLine(test.Description);
                var result = validator.GetTweetLength(test.Text);
                if (result != test.Expected)
                    Debugger.Break();
            }
        }
    }
}
