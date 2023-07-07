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
                        if (char.IsLetter(current))
                        {
                            var startIndex = pointer.Position;
                            while (pointer.Position < Source.Length && char.IsLetter(Source[pointer.Position]))
                            {
                                pointer.Position += 1;
                            }

                            var value = Source.Substring(startIndex, pointer.Position - startIndex);

                            token.Value = value;

                            if (Keywords.Contains(value.ToLower()))
                            {
                                token.TokenType = TokenType.Keyword;
                            }
                            else
                            {
                                token.TokenType = TokenType.Identifier;
                            }

                            token.Location = new Location
                            {
                                Column = col,
                                Row = row
                            };

                            col += (pointer.Position - startIndex);

                            // Re read non letter
                            pointer.Position -= 1;
                        }
                        else if (char.IsDigit(current))
                        {
                            var startIndex = pointer.Position;
                            while (pointer.Position < Source.Length && char.IsDigit(Source[pointer.Position]))
                            {
                                pointer.Position += 1;
                            }

                            var value = Source.Substring(startIndex, pointer.Position - startIndex);
                            token.Value = value;
                            token.TokenType = TokenType.Numeric;
                            token.Location = new Location
                            {
                                Column = col,
                                Row = row
                            };

                            col += (pointer.Position - startIndex);

                            // Re read non digit
                            pointer.Position -= 1;
                        }
                        else if (current == ' ')
                        {
                            col += 1;
                        }
                        else
                        {
                            Console.WriteLine($"Character: {current} not recognized. col: {col} row: {row}");
                            return new Token[0];
                        }

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
