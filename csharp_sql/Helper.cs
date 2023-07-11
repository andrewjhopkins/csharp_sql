using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_sql
{
    public  static class Helper
    {
        private static Dictionary<char, TokenType> SymbolToTokenTypeMapping = new Dictionary<char, TokenType>
        {
            { ',', TokenType.Comma },
            { '(', TokenType.LeftParen },
            { ')', TokenType.RightParen },
            { ';', TokenType.Semicolon },
        };

        public static TokenType SymbolToTokenType(char symbol)
        {
            if (SymbolToTokenTypeMapping.ContainsKey(symbol))
            { 
                return SymbolToTokenTypeMapping[symbol];
            }

            // TODO: Error handling
            throw new Exception();
        }
    }
}
