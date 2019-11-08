namespace ToriatamaText.Test.ConformanceYaml
{
    class TestItem<TExpected>
    {
        public string Description { get; set; }
        public string Text { get; set; }
        public TExpected Expected { get; set; }
    }
}
