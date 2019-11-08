using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

Directory.CreateDirectory("obj");

GenerateDefaultTlds();
GenerateUnicodeNormalizationTables();

void GenerateDefaultTlds()
{
    Console.WriteLine(nameof(GenerateDefaultTlds));

    var tldFile = Path.Combine("obj", "tld_lib.yml");
    if (!File.Exists(tldFile))
    {
        Console.WriteLine("Downloading tld_lib.yml");
        using (var client = new WebClient())
            client.DownloadFile("https://github.com/twitter/twitter-text/raw/master/conformance/tld_lib.yml", tldFile);
    }

    using (var reader = new StreamReader(tldFile))
    using (var writer = new StreamWriter("DefaultTlds.g.cs"))
    {
        writer.WriteLine("using System.Collections.Generic;");
        writer.WriteLine();
        writer.WriteLine("namespace ToriatamaText");
        writer.WriteLine('{');
        writer.WriteLine("    public static class DefaultTlds");
        writer.WriteLine("    {");

        reader.ReadLine(); // ---

        var line = reader.ReadLine();
        if (line != "country:") throw new Exception();

        writer.WriteLine("        public static IReadOnlyList<string> CTlds { get; } = new[]");
        writer.WriteLine("        {");

        Action write = () =>
        {
            writer.WriteLine(
                "            \"{0}\",",
                line[2] == '"'
                    ? line.Substring(3, line.Length - 4)
                    : line.Substring(2)
            );
        };

        while ((line = reader.ReadLine()).StartsWith("- ", StringComparison.Ordinal))
            write();

        writer.WriteLine("        };");
        writer.WriteLine();

        if (line != "generic:") throw new Exception();

        writer.WriteLine("        public static IReadOnlyList<string> GTlds { get; } = new[]");
        writer.WriteLine("        {");

        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            write();

        writer.WriteLine("        };");
        writer.WriteLine("    }");
        writer.WriteLine('}');
    }
}

class UnicodeDataRow
{
    public int Code;
    public int CanonicalCombiningClass;
    public string[] DecompositionMapping;
}

int ParseHex(string x) => int.Parse(x, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

void ForEachCodePoint(string range, Action<int> action)
{
    var s = range.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
    if (s.Length == 1)
        action(ParseHex(s[0]));
    else if (s.Length == 2)
    {
        var end = ParseHex(s[1]);
        for (var i = ParseHex(s[0]); i <= end; i++)
            action(i);
    }
    else
    {
        throw new Exception("Invalid DerivedNormalizationProps.txt");
    }
}

uint ToUtf16(int code)
{
    var s = char.ConvertFromUtf32(code);
    return s.Length == 1 ? s[0] : ((uint)s[0]) << 16 | s[1];
}

uint CompTableKeyHash(ulong key)
{
    return (uint)(key ^ (key >> 20));
}

void GenerateUnicodeNormalizationTables()
{
    Console.WriteLine(nameof(GenerateUnicodeNormalizationTables));

    var unicodeDataFile = Path.Combine("obj", "UnicodeData.txt");
    if (!File.Exists(unicodeDataFile))
    {
        Console.WriteLine("Downloading UnicodeData.txt");
        using (var client = new WebClient())
            client.DownloadFile("http://www.unicode.org/Public/UCD/latest/ucd/UnicodeData.txt", unicodeDataFile);
    }

    var normPropsFile = Path.Combine("obj", "DerivedNormalizationProps.txt");
    if (!File.Exists(normPropsFile))
    {
        Console.WriteLine("Downloading DerivedNormalizationProps.txt");
        using (var client = new WebClient())
            client.DownloadFile("http://www.unicode.org/Public/UCD/latest/ucd/DerivedNormalizationProps.txt", normPropsFile);
    }

    var data = new List<UnicodeDataRow>();
    var compositionExclusions = new List<int>();
    var nfcQcNorM = new HashSet<int>();

    using (var reader = new StreamReader(unicodeDataFile))
    {
        string line;
        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            var s = line.Split(';');
            var mapping = s[5].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            data.Add(new UnicodeDataRow
            {
                Code = ParseHex(s[0]),
                CanonicalCombiningClass = int.Parse(s[3], CultureInfo.InvariantCulture),
                DecompositionMapping = mapping.Length == 0 ? null : mapping
            });
        }
    }

    using (var reader = new StreamReader(normPropsFile))
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Length != 0 && line[0] != '#')
            {
                var s = line.Split(';');
                if (s[1].TrimStart(null).StartsWith("Full_Composition_Exclusion", StringComparison.Ordinal))
                {
                    ForEachCodePoint(s[0], compositionExclusions.Add);
                }
                else if (s[1].Trim() == "NFC_QC")
                {
                    ForEachCodePoint(s[0], x => nfcQcNorM.Add(x));
                }
            }
        }
    }

    compositionExclusions.Sort();

    var dataDic = new Dictionary<int, UnicodeDataRow>(data.Count);
    foreach (var x in data) dataDic.Add(x.Code, x);

    // Value: [マッピング1文字目(4bytes)][マッピング2文字目(4bytes)]
    var decompTableItems = new List<KeyValuePair<uint, ulong>>();

    // Key: [1文字目(4bytes)][2文字目(4bytes)]
    var compTableItems = new List<KeyValuePair<ulong, uint>>();

    var cccAndQcTableItems = new List<KeyValuePair<uint, int>>();

    foreach (var x in data)
    {
        var mapping = x.DecompositionMapping;
        if (mapping != null && mapping[0][0] == '<')
            mapping = null;

        if (mapping != null)
        {
            var value = (ulong)ToUtf16(ParseHex(mapping[0])) << 32;
            if (mapping.Length == 2)
            {
                value |= ToUtf16(ParseHex(mapping[1]));

                if (compositionExclusions.BinarySearch(x.Code) < 0)
                {
                    compTableItems.Add(new KeyValuePair<ulong, uint>(value, ToUtf16(x.Code)));
                }
            }
            else if (mapping.Length > 2)
            {
                throw new Exception($"\\u{Convert.ToString(x.Code, 16)} has too many elements in the decomposition mapping.");
            }

            decompTableItems.Add(new KeyValuePair<uint, ulong>(ToUtf16(x.Code), value));
        }

        if (x.CanonicalCombiningClass != 0 || nfcQcNorM.Contains(x.Code))
            cccAndQcTableItems.Add(new KeyValuePair<uint, int>(ToUtf16(x.Code), x.CanonicalCombiningClass));
    }

    uint decompTableCapacity = 2048;
    var decompTableBuckets = new int[decompTableCapacity];
    for (uint i = 0; i < decompTableCapacity; i++) decompTableBuckets[i] = -1;
    // 要素1: キー
    // 要素2: 次のエントリーのキーのインデックス
    // 要素3: 値（1文字目）
    // 要素4: 値（2文字目）
    var decompTableEntries = new uint[decompTableItems.Count * 4];
    var decompTableEntryIndex = 0;

    Console.WriteLine("DecompositionTable");
    foreach (var group in decompTableItems.GroupBy(x => x.Key % decompTableCapacity))
    {
        Console.Write(group.Count() + " ");
        var next = -1;
        foreach (var x in group)
        {
            decompTableEntries[decompTableEntryIndex] = x.Key;
            decompTableEntries[decompTableEntryIndex + 1] = (uint)next;
            decompTableEntries[decompTableEntryIndex + 2] = (uint)(x.Value >> 32);
            decompTableEntries[decompTableEntryIndex + 3] = (uint)x.Value;
            next = decompTableEntryIndex;
            decompTableEntryIndex += 4;
        }
        decompTableBuckets[group.Key] = next;
    }

    Console.WriteLine();
    Console.WriteLine(decompTableBuckets.LongCount(x => x == -1));

    uint compTableCapacity = 1103;
    var compTableBuckets = new int[compTableCapacity];
    for (uint i = 0; i < compTableCapacity; i++) compTableBuckets[i] = -1;
    // 要素1: キー
    // 要素2: [値(4bytes)][次のエントリーのキーのインデックス(4bytes)]
    var compTableEntries = new ulong[compTableItems.Count * 2];
    var compTableEntryIndex = 0;

    Console.WriteLine("CompositionTable");
    foreach (var group in compTableItems.GroupBy(x => CompTableKeyHash(x.Key) % compTableCapacity))
    {
        //var count = group.Count();
        //if (count > 5)
        //{
        //    Console.WriteLine("{0:X}: {1:D}件", group.Key, count);
        //    foreach (var x in group)
        //        Console.WriteLine("{0:X11}({1:X4}) -> {2:X4}", x.Key, CompTableKeyHash(x.Key), x.Value);
        //}
        Console.Write(group.Count() + " ");

        var next = -1;
        foreach (var x in group)
        {
            compTableEntries[compTableEntryIndex] = x.Key;
            compTableEntries[compTableEntryIndex + 1] = ((ulong)x.Value) << 32 | (ulong)(uint)next;
            next = compTableEntryIndex;
            compTableEntryIndex += 2;
        }
        compTableBuckets[group.Key] = next;
    }

    Console.WriteLine();
    Console.WriteLine(compTableBuckets.LongCount(x => x == -1));

    uint cccAndQcTableCapacity = 1024;
    var cccAndQcTableBuckets = new int[cccAndQcTableCapacity];
    for (uint i = 0; i < cccAndQcTableCapacity; i++) cccAndQcTableBuckets[i] = -1;
    // [value(1byte)][next(2bytes)][key(4bytes)]
    var cccAndQcTableEntries = new ulong[cccAndQcTableItems.Count];
    int cccAndQcTableEntryIndex = 0;

    Console.WriteLine("CccAndQcTable");
    foreach (var group in cccAndQcTableItems.GroupBy(x => x.Key % cccAndQcTableCapacity))
    {
        Console.Write(group.Count() + " ");
        int next = 0xFFFF;
        foreach (var x in group)
        {
            cccAndQcTableEntries[cccAndQcTableEntryIndex] = ((ulong)x.Value) << 48 | ((ulong)next) << 32 | ((ulong)x.Key);
            next = cccAndQcTableEntryIndex++;
        }
        cccAndQcTableBuckets[group.Key] = next;
    }

    Console.WriteLine();
    Console.WriteLine(cccAndQcTableBuckets.Count(x => x == -1));

    using (var writer = new StreamWriter(Path.Combine("UnicodeNormalization", "Tables.g.cs")))
    {
        writer.WriteLine("namespace ToriatamaText.UnicodeNormalization");
        writer.WriteLine('{');
        writer.WriteLine("    partial class Tables");
        writer.WriteLine("    {");
        writer.WriteLine("        private const int DecompositionTableCapacity = {0:D};", decompTableCapacity);
        writer.WriteLine();
        writer.Write("        private static readonly short[] DecompositionTableBuckets = { ");

        foreach (var x in decompTableBuckets)
            writer.Write(x.ToString("D") + ", ");

        writer.WriteLine("};");
        writer.WriteLine();
        writer.WriteLine("        // Count: {0:D} * 4 = {1:D}", decompTableItems.Count, decompTableEntries.Length);
        writer.Write("        public static readonly uint[] DecompositionTableEntries = { ");

        foreach (var x in decompTableEntries)
            writer.Write("0x{0:X4}, ", x);

        writer.WriteLine("};");
        writer.WriteLine();
        writer.WriteLine("        private const int CompositionTableCapacity = {0:D};", compTableCapacity);
        writer.WriteLine();
        writer.Write("        private static readonly short[] CompositionTableBuckets = { ");

        foreach (var x in compTableBuckets)
            writer.Write(x.ToString("D") + ", ");

        writer.WriteLine("};");
        writer.WriteLine();
        writer.WriteLine("        // Count: {0:D} * 2 = {1:D}", compTableItems.Count, compTableEntries.Length);
        writer.Write("        public static readonly ulong[] CompositionTableEntries = { ");

        foreach (var x in compTableEntries)
            writer.Write("0x{0:X12}, ", x);

        writer.WriteLine("};");
        writer.WriteLine();
        writer.WriteLine("        private const int CccAndQcTableCapacity = {0:D};", cccAndQcTableCapacity);
        writer.WriteLine();
        writer.Write("        private static readonly short[] CccAndQcTableBuckets = { ");

        foreach (var x in cccAndQcTableBuckets)
            writer.Write(x.ToString("D") + ", ");

        writer.WriteLine("};");
        writer.WriteLine();
        writer.WriteLine("        // Count: " + cccAndQcTableEntries.Length.ToString("D"));
        writer.Write("        private static readonly ulong[] CccAndQcTableEntries = { ");

        foreach (var x in cccAndQcTableEntries)
            writer.Write("0x{0:X14}, ", x);

        writer.WriteLine("};");
        writer.WriteLine("    }");
        writer.WriteLine("}");
    }
}
