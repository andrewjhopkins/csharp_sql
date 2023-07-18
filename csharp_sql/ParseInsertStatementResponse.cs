using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseInsertStatementResponse
    {
        public int NextCursor { get; set; }
        public InsertStatement InsertStatement { get; set; }
        public bool Ok { get; set; }
    }
}
