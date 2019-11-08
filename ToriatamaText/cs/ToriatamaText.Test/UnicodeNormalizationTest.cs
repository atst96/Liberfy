using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ToriatamaText.Collections;
using ToriatamaText.UnicodeNormalization;

namespace ToriatamaText.Test
{
    static class UnicodeNormalizationTest
    {
        public static void Run()
        {
            if (!File.Exists(testFile))
                DownloadTests();

            var tests = LoadTests();

            foreach (var x in tests)
            {
                // NFC
                // c2 == toNFC(c1) == toNFC(c2) == toNFC(c3)
                // c4 == toNFC(c4) == toNFC(c5)
                MiniList<char> r1;
                var s1 = NewSuperNfc.Compose(x[0], out r1) ? ToString(r1) : x[0];
                MiniList<char> r2;
                var s2 = NewSuperNfc.Compose(x[1], out r2) ? ToString(r2) : x[1];
                MiniList<char> r3;
                var s3 = NewSuperNfc.Compose(x[2], out r3) ? ToString(r3) : x[2];
                MiniList<char> r4;
                var s4 = NewSuperNfc.Compose(x[3], out r4) ? ToString(r4) : x[3];
                MiniList<char> r5;
                var s5 = NewSuperNfc.Compose(x[4], out r5) ? ToString(r5) : x[4];

                if (!(s1 == x[1] && s2 == x[1] && s3 == x[1]
                    && s4 == x[3] && s5 == x[3]))
                {
                    Debugger.Break();
                }
            }

            var stopwatch = new Stopwatch();
            const int ntimes =
#if DEBUG
                100 // Debug ビルドだと遅すぎて待ってるのだるい
#else
                300
#endif
                ;

            stopwatch.Start();
            for (var i = 0; i < ntimes; i++)
            {
                foreach (var x in tests)
                {
                    foreach (var y in x)
                    {
                        MiniList<char> result;
                        if (NewSuperNfc.Compose(y, out result))
                            ToString(result);
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine("ToriatamaText.UnicodeNormalization: {0}", stopwatch.Elapsed);

            stopwatch.Restart();
            for (var i = 0; i < ntimes; i++)
            {
                foreach (var x in tests)
                {
                    foreach (var y in x)
                        y.Normalize();
                }
            }
            stopwatch.Stop();
            Console.WriteLine("String.Normalize: {0}", stopwatch.Elapsed);
        }

        static string ToString(MiniList<int> miniList)
        {
            var sb = new StringBuilder(miniList.Count);
            for (var i = 0; i < miniList.Count; i++)
            {
                var x = miniList[i];
                if (x <= char.MaxValue)
                    sb.Append((char)x);
                else
                {
                    x -= 0x10000;
                    sb.Append((char)((x / 0x400) + 0xD800)).Append((char)((x % 0x400) + 0xDC00));
                }
            }
            return sb.ToString();
        }

        static string ToString(MiniList<char> miniList)
        {
            return new string(miniList.InnerArray, 0, miniList.Count);
        }

        private const string testFile = "NormalizationTest.txt";

        static void DownloadTests()
        {
            Console.WriteLine("Downloading NormalizationTest.txt");
            new WebClient().DownloadFile(
                "http://www.unicode.org/Public/UCD/latest/ucd/NormalizationTest.txt",
                testFile);
        }

        static string[][] LoadTests()
        {
            if (!File.Exists(testFile))
                DownloadTests();

            var tests = new List<string[]>();
            using (var sr = new StreamReader(testFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '#' || line[0] == '@') continue;
                    tests.Add(
                        line.Split(';').Take(5)
                        .Select(s => string.Concat(s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => char.ConvertFromUtf32(int.Parse(x, NumberStyles.HexNumber, CultureInfo.InvariantCulture)))
                        ))
                        .ToArray()
                    );
                }
            }
            return tests.ToArray();
        }
    }
}
