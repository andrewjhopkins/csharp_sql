namespace csharp_sql.Statements
{
    public class InsertStatement : IStatement
    {
        public Token Table { get; set; }
        public IEnumerable<Expression> Columns { get; set; }
        public IEnumerable<Expression> Values { get; set; }
    }
}
