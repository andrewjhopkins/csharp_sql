namespace csharp_sql
{
    public class Token
    {
        public TokenType TokenType { get; set; }
        public Location Location { get; set; }
        public string Value { get; set; }
    }
}
