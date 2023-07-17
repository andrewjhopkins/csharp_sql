using csharp_sql.Statements;

namespace csharp_sql
{
    public class ParseSelectItemsResponse
    {
        public IEnumerable<SelectItem> SelectItems { get; set; }
        public int NextCursor { get; set; }
        public bool Ok { get; set; }
    }
}
