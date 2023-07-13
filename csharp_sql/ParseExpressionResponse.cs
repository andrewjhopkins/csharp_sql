using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseExpressionResponse
    {
        int NextCursor { get; set; }
        Expression Expression { get; set; }
        bool Ok { get; set; }
    }
}
