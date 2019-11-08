namespace ToriatamaText.InternalExtractors
{
    enum TldType
    {
        None,
        GTld,
        CcTld,
        SpecialCcTld
    }

    struct TldInfo
    {
        public TldType Type;
        public int Length;

        public TldInfo(TldType type, int length)
        {
            this.Type = type;
            this.Length = length;
        }
    }
}
