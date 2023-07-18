namespace csharp_sql
{
    public class ParseColumnDefinitionResponse
    {
        public IEnumerable<ColumnDefinition> ColumnDefinitions { get; set; }
        public int NextCursor { get; set; }
        public bool Ok { get; set; }
    }
}
