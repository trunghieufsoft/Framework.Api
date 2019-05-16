namespace Common.DTOs.Export
{
    public class ExcelSheet
    {
        public ColumnGroup[] Columns { get; set; }
        public dynamic ListData { get; set; }
        public bool HasGroup { get; set; }
        public string SheetName { get; set; }
    }

    public class PropertyIndex
    {
        public string Property { get; set; }
        public int Index { get; set; }
    }
}