using System.Runtime.CompilerServices;

namespace ToriatamaText.UnicodeNormalization
{
    static partial class Tables
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LookupCccAndQcTable(uint key)
        {
            var i = CccAndQcTableBuckets[key % CccAndQcTableCapacity];
            while (i != -1)
            {
                var entry = CccAndQcTableEntries[i];
                if (((uint)entry) == key)
                    return true;
                i = (short)(entry >> 32);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LookupCccAndQcTable(uint key, out int ccc)
        {
            var i = CccAndQcTableBuckets[key % CccAndQcTableCapacity];
            while (i != -1)
            {
                var entry = CccAndQcTableEntries[i];
                if (((uint)entry) == key)
                {
                    ccc = (int)(entry >> 48);
                    return true;
                }
                i = (short)(entry >> 32);
            }
            ccc = 0;
            return false;
        }

        public static int GetCanonicalCombiningClass(uint code)
        {
            int v;
            LookupCccAndQcTable(code, out v);
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <returns>エントリーのインデックス</returns>
        public static int LookupDecompositionTable(uint key)
        {
            int i = DecompositionTableBuckets[key % DecompositionTableCapacity];
            while (i != -1)
            {
                if (DecompositionTableEntries[i] == key)
                    return i + 2;
                i = (int)DecompositionTableEntries[i + 1];
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LookupCompositionTable(ulong key, out uint value)
        {
            int i = CompositionTableBuckets[((uint)(key ^ (key >> 20))) % CompositionTableCapacity];
            while (i != -1)
            {
                var entryValue = CompositionTableEntries[i + 1];
                if (CompositionTableEntries[i] == key)
                {
                    value = (uint)(entryValue >> 32);
                    return true;
                }
                i = (int)entryValue;
            }
            value = 0;
            return false;
        }
    }
}
