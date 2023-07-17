namespace csharp_sql.Statements
{
    public class SelectItem
    {
        public Expression Expression { get; set; }
        public bool Asterisk { get; set; }
        public Token AsToken { get; set; }
    }
}
