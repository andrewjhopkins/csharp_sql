using csharp_sql.Statements;

namespace csharp_sql
{
    public class Parser
    {
        public IEnumerable<IStatement> Parse(IEnumerable<Token> tokens)
        {
            var ast = new List<IStatement>();

            if (tokens.Count() == 0)
            {
                return ast;
            }


            return ast;
        }
    }
}
