using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseSelectStatementResponse
    {
        public int NextCursor { get; set; }
        public SelectStatement SelectStatement { get; set; }
        public bool Ok { get; set; }
    }
}
