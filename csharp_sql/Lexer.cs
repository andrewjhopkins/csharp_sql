namespace csharp_sql
{
    public class Lexer
    {
        public string Source { get; set; }
        public Lexer(string source)
        { 
            Source = source;
        }

        public static HashSet<String> Keywords = new HashSet<string>
        {
            "select",
            "from",
            "as",
            "table",
            "create",
            "insert",
            "into",
            "values",
            "int",
            "text"
        };

        public IEnumerable<Token> Lex()
        { 
            if (string.IsNullOrEmpty(Source)) 
            {
                Console.WriteLine("No source loaded.");
                return new Token[0];
            }

            var tokens = new List<Token>();

            var row = 0;
            var col = 0;
            var pointer = new Pointer();

            while (pointer.Position < Source.Length)
            {
                var current = Source[pointer.Position];
                // Console.WriteLine($"{current} {col} {row}");
                var token = new Token();

                switch (current)
                {
                    case '\n':
                        col = 0;
                        row += 1;
                        break;
                    case ',':
                    case '(':
                    case ')':
                    case ';':
                        token.Value = $"{current}";
                        token.TokenType = TokenType.Symbol;
                        token.Location = new Location
                        {
                            Column = col,
                            Row = row,
                        };
                        col += 1;
                        break;
                    case '\'':
                        var nextQuoteIndex = Source.IndexOf('\'', pointer.Position + 1);
                        if (nextQuoteIndex < 0 || nextQuoteIndex >= Source.Length)
                        {
                            // TODO: throw error and quit
                            Console.WriteLine("Expected '");
                            return new Token[0];
                        }
                        if (nextQuoteIndex > 0 && nextQuoteIndex < Source.Length)
                        {
                            token.Value = Source.Substring(pointer.Position, nextQuoteIndex + 1 - pointer.Position);
                            token.TokenType = TokenType.String;
                        }

                        token.Location = new Location
                        {
                            Column = col,
                            Row = row,
                        };

                        col = nextQuoteIndex + 1;
                        pointer.Position = nextQuoteIndex;
                        break;
                    default:
                        // TODO: get keyword if exists else throw error
                        col += 1;
                        break;
                }

                if (token.Location != null)
                { 
                    tokens.Add(token);
                }

                pointer.Position += 1;
            }

            return tokens;
        }
    }
}
