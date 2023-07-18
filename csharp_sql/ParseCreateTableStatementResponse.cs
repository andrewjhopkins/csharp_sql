using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseCreateTableStatementResponse
    {
        public int NextCursor { get; set; }
        public CreateTableStatement CreateTableStatement { get; set; }
        public bool Ok { get; set; }
    }
}
