using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseStatementResponse
    {
        public IStatement Statement { get; set; }
        public int NextCursor { get; set; }
    }
}
