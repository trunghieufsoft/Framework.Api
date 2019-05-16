namespace Common.DTOs.Export
{
    public class ColumnGroup
    {
        public string Group { get; set; }
        public string[] Header { get; set; }
        public string[] Property { get; set; }
    }

    public class ColumnFromJson
    {
        public ColumnGroup[] Items { get; set; }
    }
}