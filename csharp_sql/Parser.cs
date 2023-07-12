using csharp_sql.Statements;

namespace csharp_sql
{
    public class Parser
    {
        public IEnumerable<IStatement> Parse(IEnumerable<Token> tokens)
        {

            if (tokens.Count() == 0)
            {
                throw new Exception();
            }

            // append semicolon if needed
            if (tokens.Last().TokenType != TokenType.Semicolon)
            {
                tokens.Concat(new Token[] { new Token { TokenType = TokenType.Semicolon, Value = ";" } });
            }

            var ast = new List<IStatement>();

            var cursor = 0;

            while (cursor < tokens.Count()) 
            { 
            }

            return ast;
        }

        public IStatement ParseStatement(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;
            var test = ParseSelectStatement(tokens, cursor);
            return test;
        }

        public SelectStatement ParseSelectStatement(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;
            return null;
        }
    }
}
