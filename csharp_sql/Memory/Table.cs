namespace csharp_sql.Memory
{
    public class Table
    {
        public List<string> Columns { get; set; }
        public List<ColumnType> ColumnTypes { get; set; }

        public List<List<string>> Rows { get; set; }

        public Table()
        { 
            Columns = new List<string>();
            ColumnTypes = new List<ColumnType>();
            Rows = new List<List<string>>();
        }
    }
}
