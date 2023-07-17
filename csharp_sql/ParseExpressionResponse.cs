using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseExpressionResponse
    {
        public int NextCursor { get; set; }
        public Expression Expression { get; set; }
        public bool Ok { get; set; }
    }
}
