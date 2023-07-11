namespace csharp_sql.Statements
{
    internal class CreateTableStatement : IStatement
    {
        public Token Name { get; set; }
        public IEnumerable<ColumnDefinition> Columns { get; set; }
    }
}
