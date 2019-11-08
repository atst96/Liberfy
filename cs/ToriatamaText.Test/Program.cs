using System;

namespace ToriatamaText.Test
{
    static class Program
    {
        static void Main(string[] args)
        {
            WriteTitle(nameof(ExtractorTest));
            Console.WriteLine();
            ExtractorTest.Run();

            Console.WriteLine();
            WriteTitle(nameof(ValidatorTest));
            Console.WriteLine();
            ValidatorTest.Run();

            Console.WriteLine();
            WriteTitle(nameof(UnicodeNormalizationTest));
            Console.WriteLine();
            UnicodeNormalizationTest.Run();

            Console.WriteLine();
            Console.WriteLine("End");
            Console.ReadLine();
        }

        static void WriteTitle(string name)
        {
            var x = new string('/', name.Length + 6);
            Console.WriteLine(x);
            Console.WriteLine("// " + name + " //");
            Console.WriteLine(x);
        }
    }
}
