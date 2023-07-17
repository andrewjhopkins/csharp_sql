using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_sql
{
    public  static class Helper
    {
        public static Dictionary<char, TokenType> SymbolToTokenTypeMapping = new Dictionary<char, TokenType>
        {
            { ',', TokenType.Comma },
            { '(', TokenType.LeftParen },
            { ')', TokenType.RightParen },
            { '*', TokenType.Asterisk },
            { ';', TokenType.Semicolon },
        };

        public static Dictionary<string, TokenType> KeywordToTokenTypeMapping = new Dictionary<string, TokenType>
        {
            { "select", TokenType.Select },
            { "from", TokenType.From },
            { "as", TokenType.As },
            { "table", TokenType.Table },
            { "create", TokenType.Create },
            { "insert", TokenType.Insert },
            { "into", TokenType.Into },
            { "values", TokenType.Values },
            { "int", TokenType.Int },
            { "text", TokenType.Text }
        };
    }
}
