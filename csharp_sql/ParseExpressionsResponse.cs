using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseExpressionsResponse
    {
        public IEnumerable<Expression> Expressions { get; set; }
        public int NextCursor { get; set; }
        public bool Ok { get; set; }
    }
}
