namespace csharp_sql.Statements
{
    public class CreateTableStatement : IStatement
    {
        public Token Name { get; set; }
        public IEnumerable<ColumnDefinition> Columns { get; set; }
    }
}
