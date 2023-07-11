namespace csharp_sql.Statements
{
    public class SelectStatement : IStatement
    {
        public IEnumerable<SelectItem> Items { get; set; }
        public FromItem From { get; set; }
    }
}
