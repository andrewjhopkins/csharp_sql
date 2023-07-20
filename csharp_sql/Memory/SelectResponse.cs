namespace csharp_sql.Memory
{
    public class SelectResponse
    {
        public IEnumerable<ResultColumn> Columns { get; set; }
        public IEnumerable<IEnumerable<Cell>> Rows { get; set; }
    }
}
