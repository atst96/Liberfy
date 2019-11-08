namespace ToriatamaText
{
    public struct EntityInfo
    {
        public int StartIndex { get; }
        public int Length { get; }
        public EntityType Type { get; }

        public EntityInfo(int startIndex, int length, EntityType type)
        {
            this.StartIndex = startIndex;
            this.Length = length;
            this.Type = type;
        }
    }
}
