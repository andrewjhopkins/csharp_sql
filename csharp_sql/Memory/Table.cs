namespace csharp_sql.Memory
{
    public class Table
    {
        public IEnumerable<string> Columns { get; set; }
        public IEnumerable<ColumnType> ColumnTypes { get; set; }

        public IEnumerable<IEnumerable<string>> Rows { get; set; }

        public Table()
        { 
            Columns = new List<string>();
            ColumnTypes = new List<ColumnType>();
            Rows = new List<List<string>>();
        }
    }
}
